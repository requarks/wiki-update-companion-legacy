using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace wiki_update_companion.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UpgradeController : ControllerBase
    {
        // GET: Upgrade
        [HttpGet]
        public string Get()
        {
            return "Hi";
        }

        // POST: Upgrade
        [HttpPost]
        public async void Post()
        {
            Uri dockerSocket = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new Uri("npipe://./pipe/docker_engine")
                : new Uri("unix:///var/run/docker.sock");

            DockerClient client = new DockerClientConfiguration(dockerSocket).CreateClient();

            CreateContainerResponse wtcontainer = await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = "containrrr/watchtower",
                Name = "watchtower",
                Cmd = new List<String>() {
                    "wiki",
                    "--cleanup",
                    "--run-once",
                    "--debug"
                },
                HostConfig = new HostConfig
                {
                    AutoRemove = true,
                    Mounts = new Mount[] {
                        new Mount() {
                            ReadOnly = true,
                            Source = "/var/run/docker.sock",
                            Target = "/var/run/docker.sock",
                            Type = "bind"
                        }
                    }
                },
                AttachStderr = true,
                AttachStdout = true
            });

            await client.Containers.StartContainerAsync(wtcontainer.ID, new ContainerStartParameters());
        }
    }
}
