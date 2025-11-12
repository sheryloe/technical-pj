using System;
using System.Collections.Generic;
using System.IO;

public class CoffeeRecipe
{
    public string Name { get; }
    public Dictionary<string, int> Usage_Ingredient { get; }

    public CoffeeRecipe(string name, Dictionary<string,int> ingredients)
    {
        this.Name = name;
        this.Usage_Ingredient = ingredients;
    }
}
public class CoffeeMachine
{
    private Dictionary<string, int> _stock;
    private Dictionary<string, CoffeeRecipe> _recipes;
    private List<string> _ingredientHeaders = new List<string>();    
    private const string RecipeFilePath = "recipes.csv";
    public CoffeeMachine(Dictionary<string, int> Stocks)
    {
        this._stock = new Dictionary<string, int>(Stocks);
        try
        {
            this._recipes = LoadRecipesFromFile(RecipeFilePath);

            string loadedRecipe = string.Join(", ", _recipes.Keys);
            Console.WriteLine($"Successfully loaded {_recipes.Count} recipes : {loadedRecipe}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to load recipes file from '{RecipeFilePath}', Check for recipes file : {ex}");
            this._recipes = new Dictionary<string, CoffeeRecipe>();
        }

        Console.WriteLine($"Coffee machine ready");
        PrintStock();
    }
    
    private Dictionary<string, CoffeeRecipe> LoadRecipesFromFile(string filePath)
    {
        var recipes = new Dictionary<string, CoffeeRecipe>();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Recipe file not found at '{filePath}'.");
        }

        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length < 2)
        {
            throw new InvalidDataException("Recipe files is Empty");
        }

        string[] headers = lines[0].Split(',');

        _ingredientHeaders = new List<string>(); 

        for (int i = 1; i < headers.Length; i++) 
        {
            // 3. 공백을 제거하고 (예: " Beans " -> "Beans")
            string trimmedHeader = headers[i].Trim(); 
        
            // 4. 리스트에 추가합니다.
            _ingredientHeaders.Add(trimmedHeader);      
        }

        foreach (string line in lines.Skip(1))
        {
            string[] parts = line.Split(',');

            try
            {
                if (parts.Length != headers.Length)
                {
                    throw new FormatException($"Line columns error check for csv file.");
                }

                string name = parts[0].Trim();
                var usage_ingredients = new Dictionary<string, int>();

                for (int i = 0; i < _ingredientHeaders.Count; i++)
                {
                    string ingredientName = _ingredientHeaders[i];
                    // parts[i + 1] : +1은 parts[0]이 "Name"이기 때문입니다.
                    int usage_amount = int.Parse(parts[i + 1].Trim());

                    usage_ingredients.Add(ingredientName, usage_amount);
                }

                var recipe = new CoffeeRecipe(name, usage_ingredients);
                if (!recipes.ContainsKey(name))
                {
                    recipes.Add(name, recipe);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RecipeLoad] Error read recipe line '{line}', {ex}.");
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

        foreach (var ingredient in recipe.Usage_Ingredient)
        {
            string ingredientName = ingredient.Key;
            int usageaAmount = ingredient.Value;
            
            if (usageaAmount <= 0) continue; 

            if (!_stock.TryGetValue(ingredientName, out int currentStock) || currentStock < usageaAmount)
            {                
                throw new InvalidOperationException($"Not enough {ingredientName.ToUpper()} to make a {coffeeType}. (Required: {usageaAmount}, Have: {currentStock})");
            }
        }

        Console.WriteLine($"Starting to make {coffeeType}........");
        foreach (var ingrdient in recipe.Usage_Ingredient)
        {
            if (ingrdient.Value > 0)
            {
                // 재고 딕셔너리에서 필요한 양만큼 차감
                _stock[ingrdient.Key] -= ingrdient.Value;
            }
        }

        Console.WriteLine($"{coffeeType} is ready!");
        PrintStock(); // 남은 재고 출력
        return coffeeType;
    }
    private void PrintStock()
    {
        string Status = string.Join(", ", _stock.Select(kv => $"{kv.Key}: {kv.Value}"));
        Console.WriteLine($"(Current ingredients: {Status})");
    }
}

// --- Example Usage ---

public class Program
{
    public static void Main(string[] args)
    {
        const string stockFilePath = "ingredients.csv";
        Console.WriteLine($"Loading ingredients ...");

        Dictionary<string, int> Stock = LoadIngredientFile(stockFilePath);

        if (Stock.Count == 0)
        {
            Console.WriteLine("Ingredients file is empty.");
            return; 
        }
        
        var machine = new CoffeeMachine(Stock);
        try
        {
            machine.MakeCoffee("Latte");
            machine.MakeCoffee("Americano");
            machine.MakeCoffee("Cappuccino");
            machine.MakeCoffee("HotChoco");
            machine.MakeCoffee("Cappuccino");
            machine.MakeCoffee("Cappuccino"); // Should succeed
            machine.MakeCoffee("Chocolate"); // Should fail due to lack of ingredients
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static Dictionary<string, int> LoadIngredientFile(string filePath)
    {
        var ingredients = new Dictionary<string, int>();
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[Error] file not found at '{filePath}'.");
            return ingredients;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            string[] headers = lines[0].Split(',');
            string[] values = lines[1].Split(',');

            if (headers.Length != values.Length)
            {
                throw new InvalidDataException($"Check for ingredients.csv file.");
            }

            for (int i = 0; i < headers.Length; i++)
            {
                string ingredientName = headers[i].Trim();
                int amount = int.Parse(values[i].Trim());

                if (!ingredients.ContainsKey(ingredientName))
                {
                    ingredients.Add(ingredientName, amount);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] read file: {ex.Message}");
            return new Dictionary<string, int>();
        }

        return ingredients;
    }
}