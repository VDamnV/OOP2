using System.Collections;

namespace lab2;

class Program
{
    static void Main()
    {
        //===================
        // Generic collection
        //===================
        Console.WriteLine(new string('=', 10));
        Console.WriteLine("Generic list of products");
        List<Product> genericList = new()
        {
            new Product(101, "Milk", "DairyCo", 25.5m, 10),
            new Product(102, "Bread", "BakeryInc", 12.3m, 20),
            new Product(103, "Cheese", "DairyCo", 55.0m, 5),
            new Product(104, "Butter", "DairyCo", 45.0m, 7)
        };

        // Adding an element
        genericList.Add(new Product(105, "Eggs", "FarmFresh", 30.0m, 12));

        // Removing an element
        genericList.RemoveAt(4);

        // Updating an element
        genericList[3] = new Product(104, "Butter", "DairyCo", 50.0m, 7);

        // Element look up
        Product? found = genericList.Find(p => p.Code == 102);
        if (found == null) Console.WriteLine("Not found");
        else Console.WriteLine(found.Info());

        Console.WriteLine(new string('=', 10));
        foreach (var item in genericList)
        {
            Console.WriteLine(item.Info());
        }

        //===================
        // Non generic collection
        //===================
        Console.WriteLine(new string('=', 10));
        Console.WriteLine("Non-generic list");
        ArrayList nonGenericList = new(4);
        foreach (var item in genericList) nonGenericList.Add(item);

        // Adding an element
        nonGenericList.Add(new Product(106, "Yogurt", "DairyCo", 15.0m, 8));

        // Removing an element
        nonGenericList.RemoveAt(4);

        // Updating an element
        nonGenericList[3] = new Product(104, "Butter", "DairyCo", 52.0m, 7);

        // Element look up + looping
        foreach (var item in nonGenericList)
        {
            if (item is Product p && p.Quantity == 7)
            {
                Console.WriteLine(p.Info());
            }
        }

        //===================
        // Array
        //===================
        Console.WriteLine(new string('=', 10));
        Console.WriteLine("Array of products");
        Product[] array = new Product[4];
        for (int i = 0; i < genericList.Count && i < array.Length; i++)
        {
            array[i] = genericList[i];
        }

        // Adding an element
        array = array.Append(new Product(107, "Juice", "FruitCo", 20.0m, 6)).ToArray();

        // Removing an element
        array = array.Where((_, index) => index != 4).ToArray();

        // Updating an element
        array[3] = new Product(104, "Butter", "DairyCo", 55.0m, 7);

        // Element look up + looping
        foreach (var item in array)
        {
            if (item.Name == "Cheese")
            {
                Console.WriteLine(item.Info());
            }
        }

        nonGenericList.Clear();
        array = Array.Empty<Product>();

        //===================
        // Binary tree
        //===================
        Console.WriteLine(new string('=', 10));
        Console.WriteLine("Binary tree of products");
        var binaryTree = new BinaryTree<Product>(genericList);
        binaryTree.Insert(new Product(108, "Ham", "MeatCo", 60.0m, 4));
        binaryTree.Insert(new Product(100, "Butter", "DairyCo", 50.0m, 6));

        foreach (var item in binaryTree)
        {
            Console.WriteLine(item.Info());
        }
    }
}