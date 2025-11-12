using System;
using System.Collections.Generic;
using System.IO;

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
    private const string RecipeFilePath = "recipes.csv";
    public CoffeeMachine(int beans, int water, int milk)
    {

        this._beans = beans;
        this._water = water;
        this._milk  = milk;
        try
        {
            this._recipes = LoadRecipesFromFile(RecipeFilePath);

            string loadedRecipe = string.Join(", ", _recipes.Keys);
            Console.WriteLine($"Successfully loaded {_recipes.Count} recipes : {loadedRecipe}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to load recipes file from '{RecipeFilePath}', Check for recipes file ");
            this._recipes = new Dictionary<string, CoffeeRecipe>();
        }

        Console.WriteLine($"Coffee machine ready (Beans: {_beans}, Water: {_water}, Milk: {_milk})");
    }
    
    private Dictionary<string, CoffeeRecipe> LoadRecipesFromFile(string filePath)
    {
        var recipes = new Dictionary<string, CoffeeRecipe>();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Recipe file not found at '{filePath}'.");
        }

        IEnumerable<string> lines = File.ReadLines(filePath).Skip(1);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');

            try
            {
                if (parts.Length != 4)
                {
                    throw new FormatException("check for recipes file");
                }

                string name = parts[0].Trim();
                int usage_beans = int.Parse(parts[1].Trim());
                int usage_water = int.Parse(parts[2].Trim());
                int usage_milk = int.Parse(parts[3].Trim());

                var recipe = new CoffeeRecipe(name, usage_beans, usage_water, usage_milk);

                if (!recipes.ContainsKey(name))
                {
                    recipes.Add(name, recipe);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RecipeLoad] Error read recipe line '{line}'.");
            }
        }

        return recipes;
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