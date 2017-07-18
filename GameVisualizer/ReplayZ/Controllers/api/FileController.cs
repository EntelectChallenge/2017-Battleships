using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReplayZ.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReplayZ.Controllers.api
{
    [Route("api/[controller]/[action]/{*value}")]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;

        public FileController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public List<Folder> List(string value)
        {
            var path = $"{_appEnvironment.WebRootPath}\\Replays\\{value}\\".Replace("/", "\\");
            if (!Directory.GetDirectories(path).Any())
                return new List<Folder>();
            var files = Directory.GetDirectories(path)
                .Select(a => new Folder()
                {
                    FolderName = a.Substring(path.Length, a.Length - path.Length),
                    TotalFoldersInFolder = Directory.GetDirectories(a).Count(),
                    TotalFilesInFolder = Directory.GetFiles(a).Count(),
                }).ToList();
            
            if (string.IsNullOrWhiteSpace(value))
            {
                files = files
                .OrderByDescending(a => a.Numeric1)
                .ThenByDescending(a => a.Numeric2)
                .ThenByDescending(a => a.Numeric3)
                .ThenByDescending(a => a.Numeric4).ToList();
                foreach (var folder in files)
                {
                    try
                    {
                        folder.GameState = GetGamePoints(folder.FolderName);
                    }
                    catch (System.Exception)
                    {
                        // ignored
                    }
                }
            }else
            {
                files = files
                .OrderBy(a => a.Numeric1)
                .ThenBy(a => a.Numeric2)
                .ThenBy(a => a.Numeric3)
                .ThenBy(a => a.Numeric4).ToList();
            }
            return files;
        }

        [HttpGet]
        public string Read(string value)
        {
            var path = $"{_appEnvironment.WebRootPath}\\Replays\\{value}".Replace("/", "\\");
            return System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : "";
        }

        [HttpGet]
        public GameState GetGamePoints(string value)
        {
            var gameList = List(value);
            var roundFolder = $"{value}\\{gameList.Last().FolderName}";
            var state = JsonConvert.DeserializeObject<GameState>(Read($"{roundFolder}\\state.json"));
            state.FolderName = value;
            return state;
        }
    }
}
