using System.Collections.Generic;
using System.IO.Compression;

class Room
{
	// Private fields
	private bool locked;
	private string name;
	private string description;
	private Inventory chest;
	private Dictionary<string, Room> exits; // stores exits of this room.

	public string Name
	{
		get { return name; }
	}

	public Inventory Chest
	{
		get { return chest; }
	}

	// Create a room described "description". Initially, it has no exits.
	// "description" is something like "in a kitchen" or "in a court yard".
	public Room(string roomName, string desc)
	{
		// a Room can handle a big Inventory.
		chest = new Inventory(999999);

		locked = false;
		name = roomName;
		description = desc;
		exits = new Dictionary<string, Room>();
	}

	// Define an exit for this room.
	public void AddExit(string direction, Room neighbor)
	{
		exits.Add(direction, neighbor);
	}

	// Lock and unlock rooms
	public bool IsLocked() {
		return locked;
	}

	public void Lock() {
		locked = true;
	}

	public void Unlock() {
		locked = false;
	}

	// Return the description of the room.
	public string GetShortDescription()
	{
		return description;
	}

	// Return a long description of this room, in the form:
	//     You are in the kitchen.
	//     Exits: north, west
	public string GetLongDescription()
	{
		return $"You are now {description}.\n{GetExitString()}";
	}

	// Return the room that is reached if we go from this room in direction
	// "direction". If there is no room in that direction, return null.
	public Room GetExit(string direction)
	{
		if (exits.ContainsKey(direction))
		{
			return exits[direction];
		}
		return null;
	}

	// Return a string describing the room's exits, for example
	// "Exits: north, west".
	private string GetExitString()
	{
		string str = "Exits:";

		// Build the string in a `foreach` loop.
		// We only need the keys.
		int countCommas = 0;
		foreach (string key in exits.Keys)
		{
			if (countCommas != 0)
			{
				str += ",";
			}
			str += " " + key;
			countCommas++;
		}

		return str;
	}
}
