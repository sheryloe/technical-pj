using System;
using System.Collections.Generic;

public class CoffeeRecipe
{
    public string Name { get; }
    public int Usage_Beans { get; }
    public int Usage_Water { get; }
    public int Usage_Milk { get; }

    public CoffeeRecipe(string name, int beans, int water, int milk)
    {
        this.Name = name;
        this.Usage_Beans = beans;
        this.Usage_Water = water;
        this.Usage_Milk = milk;
    }
}
public class CoffeeMachine
{
    private int _beans;
    private int _water;
    private int _milk;

    private Dictionary<string, CoffeeRecipe> _recipes;

    public CoffeeMachine(int beans, int water, int milk)
    {
        this._beans = beans;
        this._water = water;
        this._milk = milk;

        _recipes = new Dictionary<string, CoffeeRecipe>();
        _recipes.Add("Americano", new CoffeeRecipe("Americano", beans: 1, water: 2, milk: 0));
        _recipes.Add("Latte", new CoffeeRecipe("Latte", beans: 1, water: 0, milk: 2));
        _recipes.Add("Cappuccino", new CoffeeRecipe("Latte", beans: 2, water: 1, milk: 1));

        Console.WriteLine($"Coffee machine ready (Beans: {_beans}, Water: {_water}, Milk: {_milk})");
    }

    public string MakeCoffee(string coffeeType)
    {
        // This method for making coffee has a design flaw:
        // it must be modified every time a new coffee type is introduced.
        Console.WriteLine($"\n> Order received for '{coffeeType}'");

        if (!_recipes.TryGetValue(coffeeType, out CoffeeRecipe recipe))
        {
            throw new ArgumentException($"'{coffeeType}'is not a supported menu.");
        }

        if (this._beans < recipe.Usage_Beans)
        {
            throw new InvalidOperationException($"Not enough Beans to make a {coffeeType}.");
        }
        if (this._beans < recipe.Usage_Water)
        {
            throw new InvalidOperationException($"Not enough Water to make a {coffeeType}.");
        }
        if (this._beans < recipe.Usage_Milk)
        {
            throw new InvalidOperationException($"Not enough Milk to make a {coffeeType}.");
        }

        Console.WriteLine($"Starting to make {coffeeType}........");
        this._beans -= recipe.Usage_Beans;
        this._water -= recipe.Usage_Water;
        this._milk -= recipe.Usage_Milk;

        Console.WriteLine($"'{coffeeType} is ready! (Remaining ingredients: Beans {_beans}, Water {_water}, Milk {_milk})");
        return coffeeType;
        
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
            machine.MakeCoffee("Cappuccino"); 
            machine.MakeCoffee("Latte"); 
            machine.MakeCoffee("Cappuccino");
            machine.MakeCoffee("Cappuccino"); // Should succeed
            machine.MakeCoffee("Cappuccino"); // Should fail due to lack of ingredients
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}