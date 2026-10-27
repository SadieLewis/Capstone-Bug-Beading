using BugBeading.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugBeading.Data
{
    public static class DbInitializer
    {
        public static void Initialize(InventoryContext context)
        {
            // Avoid inserting duplicate sample products.
            if (context.Products.Any())
            {
                return;
            }

            var products = new Product[]
            {
                new Product
                {
                    Name = "Purple Keychain",
                    ProductType = "Keychain",
                    DateAdded = new DateTime(2026, 1, 10),
                    Description = "A handmade purple beaded keychain featuring amethyst.",
                    Price = 12.30,
                    ImageUrl = "https://i.etsystatic.com/67630125/r/il/936bab/8522292559/il_1588xN.8522292559_kkrg.jpg"
                },

                new Product
                {
                    Name = "Teal and Brown Keychain",
                    ProductType = "Keychain",
                    DateAdded = new DateTime(2026, 1, 15),
                    Description = "A handmade keychain featuring teal and brown beads.",
                    Price = 12.30,
                    ImageUrl = "https://i.etsystatic.com/67630125/r/il/ae4845/8474370476/il_1588xN.8474370476_38be.jpg"
                },

                new Product
                {
                    Name = "Ocean Keychain",
                    ProductType = "Keychain",
                    DateAdded = new DateTime(2026, 2, 1),
                    Description = "An ocean-inspired beaded keychain.",
                    Price = 12.30,
                    ImageUrl = "https://i.etsystatic.com/67630125/r/il/0c4645/8474354618/il_1588xN.8474354618_qhf2.jpg"
                },

                new Product
                {
                    Name = "Flower Earrings with Leaf",
                    ProductType = "Earrings",
                    DateAdded = new DateTime(2026, 2, 10),
                    Description = "Handmade flower earrings with a leaf detail.",
                    Price = 10.30,
                    ImageUrl = "https://i.etsystatic.com/67630125/r/il/654672/8544205629/il_1588xN.8544205629_761p.jpg"
                },

                new Product
                {
                    Name = "Cascading Flower Earrings",
                    ProductType = "Earrings",
                    DateAdded = new DateTime(2026, 2, 15),
                    Description = "Blue, green, and yellow cascading flower earrings.",
                    Price = 10.30,
                    ImageUrl = "https://i.etsystatic.com/67630125/r/il/0be01b/8544179263/il_1588xN.8544179263_ckwr.jpg"
                },

                new Product
                {
                    Name = "Rose Quartz Earrings",
                    ProductType = "Earrings",
                    DateAdded = new DateTime(2026, 3, 1),
                    Description = "Earrings featuring rose quartz beads with silver findings.",
                    Price = 8.30,
                    ImageUrl = "https://i.etsystatic.com/67630125/r/il/74a057/8474402208/il_1588xN.8474402208_mte6.jpg"
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}