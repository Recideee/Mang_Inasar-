using System.Text;

namespace Mang_Inasar_ConsoleApp
{
    internal class Program
    {
            class PaidOrder
        {
            public string OrderId = "";
            public string Status = "PAID";
            public List<string> ItemNames = new List<string>();
            public List<decimal> UnitPrices = new List<decimal>();
            public List<int> Quantities = new List<int>();
            public List<decimal> Subtotals = new List<decimal>();
            public decimal Total;
            public decimal Cash;
            public decimal Change;
        }
        class InputClosedException : Exception
        {
        }

        enum PaymentResult
        {
            Paid,
            Cancelled,
            BackToOrder
        }

        class Program
        {
            // =====================================================================
            //  1. SETTINGS AND MENU DATA
            // =====================================================================
            const string RecordsFilePath = @"C:\Users\AJ\Downloads\MangInsar.csv";

            // General settings
            const string RestaurantName = "MANG INASAR";
            const string Currency = "PHP";
            const int MaxQuantity = 99;
            const decimal MaxCash = 9999999.99m;

            // Cashier login
            const string CashierUsername = "MangInasar123";
            const string CashierPassword = "MangInasar111326";
            const int MaxLoginAttempts = 3;

            // Screen layout
            const int BoxInner = 62;       

            // CSV format
            const string CsvHeader = "OrderId,Status,ItemName,UnitPrice,Quantity,Subtotal,Total,Cash,Change";
            static readonly string[] categoryNames =
            {
            "IHAW-SARAP SAVERS With Drinks",
            "GRILLED PORK With Drinks",
            "WHAT'S NEW? Solo Fiesta / AlmuSOLB With Drinks",
            "CHICKEN INASAL With Drinks",
            "PALABOK",
            "HALO-HALO"
            };

            static readonly string[] menuNames =
            {
            "Chicken Inasal Regular",       //  Ihaw-Sarap Savers
            "Pork Sisig",                   //  Ihaw-Sarap Savers
            "Bangus Sisig",                 //  Ihaw-Sarap Savers
            "Pork BBQ",                     //  Grilled Pork 
            "Grilled Liempo",               //  Grilled Pork 
            "Solo Fiesta: 2pc Pork BBQ",    //  What's New 
            "Chicken Inasal Paa",           //  What's New
            "Paa Large Solo",               //  Chicken Inasal
            "Pecho Large",                  //  Chicken Inasal
            "Family Size",                  //  Palabok 
            "Extra Creamy Halo-Halo",       //  Halo-Halo 
            "Crema de Leche Halo-Halo"      //  Halo-Halo
            };

            static readonly decimal[] menuPrices =
            {
            124.00m, //  Ihaw-Sarap Savers
            124.00m, //  Ihaw-Sarap Savers
            154.00m, //  Ihaw-Sarap Savers
            124.00m, //  Grilled Pork
            154.00m, //  Grilled Pork
            214.00m, //  What's New 
            219.00m, //  What's New 
            154.00m, //  Chicken Inasal
            179.00m, //  Chicken Inasal
            305.00m, //  Palabok
            39.00m,  //  Halo-Halo
            39.00m   //  Halo-Halo
            };

            // =====================================================================
            //  2. ENTRY POINT AND MAIN MENU
            // =====================================================================

            static void Main()
            {
                Console.OutputEncoding = Encoding.UTF8;

                try
                {
                    RunMainMenu();
                }
                catch (InputClosedException)
                {
                    Console.WriteLine();
                    Console.WriteLine(" Input closed. Goodbye!");
                }
            }

            static void RunMainMenu()
            {
                bool running = true;
                string message = "";

                while (running)
                {
                    ShowMainMenu(message);
                    message = "";

                    string choice = ReadInput("Enter your choice").ToUpperInvariant();

                    switch (choice)
                    {
                        case "1":
                            RunCustomerView();
                            break;
                        case "2":
                            if (CashierLogin())
                            {
                                RunCashierView();
                            }
                            break;
                        case "3":
                            running = false;
                            break;
                        default:
                            message = Error("Invalid choice. Please enter 1, 2 or 3.");
                            break;
                    }
                }

                ShowMessageBox("Thank you! Goodbye.", waitForEnter: false);
            }

            static void ShowMainMenu(string message)
            {
                BeginScreen();
                DrawBanner(RestaurantName, "Order here. Ihaw Sarap Unli Sarap!");
                DrawOptions(
                    "   [1]  Customer View",
                    "   [2]  Cashier View",
                    "   [3]  Exit");
                EndScreen(message);
            }

            // =====================================================================
            //  3. CUSTOMER VIEW
            // =====================================================================

            static void RunCustomerView()
            {
                ClearCart();
                bool inCustomerView = true;
                string message = "";

                while (inCustomerView)
                {
                    ShowCategoryScreen(message);
                    message = "";

                    string choice = ReadInput("Enter category number").ToUpperInvariant();

                    if (choice == "C")
                    {
                        // ReviewOrder returns true when the customer is finished (paid or cancelled).
                        inCustomerView = !ReviewOrder();
                    }
                    else if (choice == "X")
                    {
                        // An empty cart has nothing to lose, so no confirmation is needed.
                        if (CountCartItems() == 0 || ConfirmCancel())
                        {
                            inCustomerView = false;
                        }
                    }
                    else if (TryGetCategoryIndex(choice, out int categoryIndex))
                    {
                        inCustomerView = !RunCategory(categoryIndex);
                    }
                    else
                    {
                        message = Error("Invalid choice. Enter a category number 1-" + categoryNames.Length
                                        + ", C to review, or X to cancel.");
                    }
                }
            }

            /// <summary>
            /// Item list for one category. Returns true when the customer is finished
            /// (paid or cancelled), false to go back to the category list.
            /// </summary>
            static bool RunCategory(int categoryIndex)
            {
                string message = "";

                while (true)
                {
                    ShowItemScreen(categoryIndex, message);
                    message = "";

                    string choice = ReadInput("Enter item number").ToUpperInvariant();

                    if (choice == "B")
                    {
                        return false;
                    }

                    if (choice == "C")
                    {
                        if (ReviewOrder())
                        {
                            return true;
                        }
                    }
                    else if (TryGetItemInCategory(choice, categoryIndex, out int itemIndex))
                    {
                        message = AddItemToCart(itemIndex);
                    }
                    else
                    {
                        message = Error("Invalid choice. Enter one of the item numbers shown, B to go back, or C to review.");
                    }
                }
            }
        }
    }
}