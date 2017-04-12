using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using ReferenceBot.Domain.Command;
using ReferenceBot.Domain.Command.Ship;
using ReferenceBot.Domain.State;
using ReferenceBot.Strategy;

namespace ReferenceBot
{
    public class Bot
    {
        protected string WorkingDirectory { get; set; }
        protected string Key { get; set; }

        private const string CommandFileName = "command.txt";

        private const string PlaceShipFileName = "place.txt";

        private const string StateFileName = "state.json";

        public Bot(string key, string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            Key = key;
        }

        public void Execute()
        {
            var state = JsonConvert.DeserializeObject<GameState>(LoadState());

            int phase = state.Phase;

            if (phase == 1)
            {
                var placeShips = PlaceShips(state);
                WritePlaceShips(placeShips);
            }
            else
            {
                var move = MakeMove(state);
                WriteMove(move);
            }
        }

        private PlaceShipCommand PlaceShips(GameState gameState)
        {
            return new RandomPlacementStrategy().GetShipPlacement(gameState);
        }

        private Command MakeMove(GameState state)
        {
            return new BasicShootStrategy().ExecuteStrategy(state);
        }

        private string LoadState()
        {
            var filename = Path.Combine(WorkingDirectory, StateFileName);
            try
            {
                string jsonText;
                using (var file = new StreamReader(filename))
                {
                    jsonText = file.ReadToEnd();
                }

                return jsonText;
            }
            catch (IOException e)
            {
                Log($"Unable to read state file: {filename}");
                var trace = new StackTrace(e);
                Log($"Stacktrace: {trace}");
                return null;
            }
        }

        private void WriteMove(Command command)
        {
            var filename = Path.Combine(WorkingDirectory, CommandFileName);

            try
            {
                using (var file = new StreamWriter(filename))
                {
                    file.WriteLine(command);
                }

                Log("Command: " + command);
            }
            catch (IOException e)
            {
                Log($"Unable to write command file: {filename}");

                var trace = new StackTrace(e);
                Log($"Stacktrace: {trace}");
            }
        }

        private void WritePlaceShips(PlaceShipCommand placeShipCommand)
        {
            var filename = Path.Combine(WorkingDirectory, PlaceShipFileName);
            try
            {
                using (var file = new StreamWriter(filename))
                {
                    file.WriteLine(placeShipCommand);
                }

                Log("Placeship command: " + placeShipCommand);
            }
            catch (IOException e)
            {
                Log($"Unable to write place ship command file: {filename}");

                var trace = new StackTrace(e);
                Log($"Stacktrace: {trace}");
            }

        }

        private void Log(string message)
        {
            Console.WriteLine("[BOT]\t{0}", message);
        }
    }
}