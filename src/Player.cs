using System.IO.Compression;

class Player
{
    // auto property
    public Room CurrentRoom { get; set; }

    // fields
    private string Name;
    private int MaxHealth;
    private int Health;
    private Inventory Backpack;

    // constructor
    public Player(string name, int maxHealth)
    {
        // 25kg is pretty heavy to carry around all day
        Backpack = new Inventory(25);

        CurrentRoom = null;
        Name = name;
        MaxHealth = maxHealth;
        Health = MaxHealth;
    }

    // methods
    public bool TakeFromChest(string itemName)
    {
        Item item = CurrentRoom.Chest.Get(itemName);

        if (item != null)
        {
            if (Backpack.Put(itemName, item))
            {
                Console.WriteLine($"Picked up {itemName}.");
            }
            else
            {
                CurrentRoom.Chest.Put(itemName, item);

                Console.WriteLine($"Failed to pick up {itemName}.");
            }
        }
        else
        {
            Console.WriteLine($"There is no item called {itemName} in this room.");

            return true;
        }

        return false;
    }

    public bool DropToChest(string itemName)
    {
        Item item = Backpack.Get(itemName);

        if (item != null)
        {
            if (CurrentRoom.Chest.Put(itemName, item))
            {
                Console.WriteLine($"Dropped {itemName}.");
            }
            else
            {
                Backpack.Put(itemName, item);

                Console.WriteLine($"Failed to drop {itemName}.");
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    public string GetItems()
    {
        return Backpack.Show();
    }

    public int GetWeight()
    {
        return Backpack.TotalWeight();
    }

    public void Damage(int amount)
    {
        int health = Math.Max(Health - amount, 0);
        Console.WriteLine($"Ouch, You just took {Health - health} damage!");

        Health = health;
    }

    public void Heal(int amount)
    {
        Health = Math.Min(Health + amount, MaxHealth);
    }

    public int GetHealth()
    {
        return Health;
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public bool HasItem(string itemName) {
        Item item = Backpack.Get(itemName);

        if (item != null) {
            Backpack.Put(itemName, item);

            return true;
        }

        return false;
    }

    public string Use(string itemName, string target)
    {
        if (!HasItem(itemName)) {return "You don't have this item in your backpack!";}
        
        switch (itemName)
        {
            case "key":
                return Key(target);
            case "backpack":
                return backpack();
            case "medkit":
                return Medkit();
            default:
                return "You don't have this item.";
        }
    }

    public string Key(string target)
    {
        Backpack.Get("key");

        Room room = CurrentRoom.GetExit(target);

        if (room == null)
        {
            return "Couldn't find the room to use the key on.";
        }
        else if (room.IsLocked() == false)
        {
            return "Can't use key on this room.\nThis room is already unlocked.";
        }

        room.Unlock();

        return $"You've unlocked the {room.GetShortDescription()} door!";
    }

    public string backpack()
    {
        return "You look in your backpack and feel relieved.\nNow you just need to escape!";
    }

    public string Medkit()
    {
        Backpack.Get("medkit");

        Heal(50);

        return "You've healed 50 health!";
    }
}