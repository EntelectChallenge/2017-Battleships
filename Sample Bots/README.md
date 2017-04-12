# Sample Bots

Entelect will provide Sample Bots for C# and Java. Sample bots are basic started projects that can read game files and make random moves. Reference bots that are capable of playing a game from start to finish are also included for contestants wishing to have something more to work from.

For any additional languages we will be relying on the community contributing a sample bot for the language of their choice. If you would like your language to be supported you will have to submit a sample bot. Sample bot submissions will close at Midnight on the 26th of May, after this no additional sample bots will be accepted.

Calibration bots will be included into the game engine before the first battle (after sample bot submission have closed) and will be based on the sample bot for each language.

## Sample Bot Submissions

Follow these steps to submit a sample bot.

1. Clone this repository.
1. Create the sample bot in the language of your choice.
1. Include a readme of any environment configuration and setup guide for your language on the tournament server.
1. Create a new pull request for your sample bot.
1. After peer review we will consider your sample bot for inclusion in the tournament.
1. If the pull request is merged then the sample bot and language can be considered officially included and supported for the tournament.

### Sample Bot Submission Rules

Please ensure your sample bot follow these rules:

1. Has a `bot.json` file.
1. Can compile on any system and is in running order (Should not produce any errors when executing).
1. Reads in the arguments from the game engine.
1. Reads in the `state.json` file and parses that to a structure supported in your language.
1. Writes a ship placement file (The ship positions can be hard coded).
