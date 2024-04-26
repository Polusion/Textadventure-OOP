class Inventory
{
    // fields
    private int maxWeight;
    private Dictionary<string, Item> items;

    // constructor
    public Inventory(int maxWeight)
    {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();
    }

    // methods
    public int TotalWeight()
    {
        int total = 0;

        foreach (Item item in items.Values)
        {
            total += item.Weight;
        }

        return total;
    }

    public int FreeWeight()
    {
        return maxWeight - TotalWeight();
    }

    public bool Put(string itemName, Item item)
    {
        if (item.Weight > FreeWeight())
        {
            Console.WriteLine("You are too heavy to pick up this item.");

            return false;
        }

        items.Add(itemName, item);

        return true;
    }

    public string Show()
    {
        string str = "";
        int i = 0;

        foreach (string itemName in items.Keys)
        {
            str += itemName;
            // Add a comma after every element in the List, but not the last.
            if (i < items.ToArray().Length - 1)
            {
                str += ", ";
            }

            i++;
        }

        return str;
    }

    public Item Get(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            Item item = items[itemName];

            items.Remove(itemName);

            return item;
        }

        return null;
    }
}