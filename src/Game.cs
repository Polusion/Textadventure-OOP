using System;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private Random random = new Random();
	private bool caught = false;
	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player("Player", 100);
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside", "outside the main entrance of the university");
		Room theatre = new Room("theatre", "in a lecture theatre");
		Room pub = new Room("pub", "in the campus pub");
		Room lab = new Room("lab", "in a computing lab");
		Room storage = new Room("storage room", "you are in the storage room");
		Room office = new Room("office", "in the computing admin office");

		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);
		lab.AddExit("down", storage);

		storage.AddExit("up", lab);

		office.AddExit("west", lab);

		// Set up rooms
		office.Lock();

		// Items
		Item officeKey = new Item(1, "office key");
		Item medkit = new Item(3, "medkit");
		Item backpack = new Item(25, "a red backpack");

		// Add Items to Rooms
		storage.Chest.Put("key", officeKey);
		theatre.Chest.Put("medkit", medkit);
		office.Chest.Put("backpack", backpack);

		// Start game outside
		player.CurrentRoom = outside;
	}

	//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished)
		{
			if (caught) {
				Console.WriteLine("You got caught by the teacher!");

				break;
			}
			if (!player.IsAlive())
			{
				Console.WriteLine("You died!");

				break;
			}

			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine("\nWelcome to Zuul!");
		Console.WriteLine("It's summer vacation and the university you're attending is closed.");
		Console.WriteLine("You forgot your backpack with all your valuable items in the university!");
		Console.WriteLine("Objective: retrieve your backpack with all your stuff.");
		Console.WriteLine("Type 'help' if you need help.\n");

		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
	private bool ProcessCommand(Command command)
	{
		bool wantToQuit = false;

		if (command.IsUnknown())
		{
			Console.WriteLine("I don't know what you mean...");
			return wantToQuit; // false
		}

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				return GoRoom(command);
			case "quit":
				wantToQuit = true;
				break;
			case "look":
				Look();
				break;
			case "status":
				PrintStatus();
				break;
			case "drop":
				Drop(command);
				break;
			case "take":
				Take(command);
				break;
			case "use":
				Use(command);
				break;
			case "inspect":
				Inspect();
				break;
		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################

	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintStatus()
	{
		Console.WriteLine("You have " + player.GetHealth() + "hp.");

		if (player.GetItems().Length > 0)
		{
			Console.WriteLine($"Your backpack holds the items: {player.GetItems()}");
			Console.WriteLine($"You weigh {player.GetWeight()}kg / 25kg.");
		}
		else
		{
			Console.WriteLine("Your backpack is empty.");
		}
	}

	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.\n");

		// let the parser print the commands
		parser.PrintValidCommands();
	}

	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	private void Inspect()
	{
		string items = player.CurrentRoom.Chest.Show();

		Console.WriteLine(
			items.Length > 0 ?
			"In this room you find the items: " + items :
			"There are no items in this room."
		);
	}

	private void Take(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Take what item?");

			return;
		}

		player.TakeFromChest(command.SecondWord);
	}

	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Drop what item?");

			return;
		}

		player.DropToChest(command.SecondWord);
	}

	private void Use(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Use what item?");
			return;
		}

		string action = player.Use(command.SecondWord, command.ThirdWord);
		Console.WriteLine(action);
	}

	private bool GoRoom(Command command)
	{
		if (!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return false;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to " + direction + "!");
			return false;
		}
		else if (nextRoom.IsLocked())
		{
			Console.WriteLine("You can't enter this room. This room is locked.");
			return false;
		}

		// 25% chance to hurt the player
		if (random.Next(1, 5) == 1)
		{
			player.Damage(25);
			Console.WriteLine("You scratched your knee crawling on the floor");
		}

		player.CurrentRoom = nextRoom;

		if (nextRoom.Name == "pub") {
			Console.WriteLine("There is a teacher in the pub!");
			Console.WriteLine("Quick run before he catches you!");
			
			new Thread(() => 
			{
    			Thread.CurrentThread.IsBackground = true; 
   				
				Thread.Sleep(3000);
				
    			if (player.CurrentRoom.Name == "pub") {
					Console.WriteLine("You have been caught by the teacher!");

					caught = true;
				}
			}).Start();

			return false;
		}

		if (nextRoom.Name == "outside" && player.HasItem("backpack"))
		{
			Console.WriteLine("You've succesfully escaped with your backpack!");
			Console.WriteLine("You have completed the objective and thus beat the game, Congratulations.");

			return true;
		}

		Console.WriteLine($"You snuck into the {nextRoom.Name}.");
		Console.WriteLine(nextRoom.GetLongDescription());

		return false;
	}
}