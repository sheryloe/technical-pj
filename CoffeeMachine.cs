using System;

public class CoffeeMachine
{
    private int _beans;
    private int _water;
    private int _milk;

    public CoffeeMachine(int beans, int water, int milk)
    {
        this._beans = beans;
        this._water = water;
        this._milk = milk;
        Console.WriteLine($"Coffee machine ready (Beans: {_beans}, Water: {_water}, Milk: {_milk})");
    }

    public string MakeCoffee(string coffeeType)
    {
        // This method for making coffee has a design flaw:
        // it must be modified every time a new coffee type is introduced.
        Console.WriteLine($"\n> Order received for '{coffeeType}'");

        if (coffeeType == "Americano")
        {
            // Americano Recipe: 1 bean, 2 water
            if (this._beans < 1 || this._water < 2)
            {
                throw new InvalidOperationException("Not enough ingredients to make an Americano.");
            }
            
            Console.WriteLine("Starting to make Americano...");
            this._beans -= 1;
            this._water -= 2;
            
            Console.WriteLine($"✅ Americano is ready! (Remaining ingredients: Beans {_beans}, Water {_water}, Milk {_milk})");
            return "Americano";
        }
        else if (coffeeType == "Latte")
        {
            // Latte Recipe: 1 bean, 2 milk
            if (this._beans < 1 || this._milk < 2)
            {
                throw new InvalidOperationException("Not enough ingredients to make a Latte.");
            }

            Console.WriteLine("Starting to make Latte...");
            this._beans -= 1;
            this._milk -= 2;

            Console.WriteLine($"✅ Latte is ready! (Remaining ingredients: Beans {_beans}, Water {_water}, Milk {_milk})");
            return "Latte";
        }
        else
        {
            throw new ArgumentException($"'{coffeeType}' is not a supported menu item.");
        }
    }
}

// --- Example Usage ---

public class Program
{
    public static void Main(string[] args)
    {
        var machine = new CoffeeMachine(beans: 10, water: 10, milk: 10);
        try
        {
            machine.MakeCoffee("Latte");
            machine.MakeCoffee("Americano");
            machine.MakeCoffee("Latte"); // Should succeed
            machine.MakeCoffee("Latte"); // Should fail due to lack of ingredients
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}