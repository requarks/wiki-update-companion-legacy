using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public async void Post([FromBody] string value)
        {
            DockerClient client = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

            await client.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    Repo = "containrrr/watchtower",
                    Tag = "latest"
                },
                null,
                null
            );

            await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = "containrrr/watchtower",
                Name = "watchtower",
                Cmd = new String[] {
                    "--cleanup",
                    "--run-once",
                    "wiki"
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
                }
            });

            await client.Containers.StartContainerAsync("watchtower", null);
        }
    }
}
