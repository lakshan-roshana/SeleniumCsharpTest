using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace SauceBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Starting Selenium Test ---");

            // 1. SETUP: Open a new Chrome Browser window
            // This creates a "Driver" object that controls the browser
            IWebDriver driver = new ChromeDriver();

            // Create a "Waiter" that waits up to 10 seconds max
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 2. NAVIGATE: Go to the SauceDemo website
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            
            // Maximizing the window so we can see everything clearly
            driver.Manage().Window.Maximize();
            Console.WriteLine("Opened Website...");

            // 3. INTERACT: Find the Username box and type 'problem_user'
            // We use By.Id because the developer gave this element an ID of "user-name"
            //IWebElement usernameField = driver.FindElement(By.Id("user-name"));

            //try Xpath instead of Id
            IWebElement usernameField = driver.FindElement(By.XPath("//input[@id='user-name']"));
            usernameField.SendKeys("problem_user");

            // 4. INTERACT: Find the Password box and type 'secret_sauce'
            //IWebElement passwordField = driver.FindElement(By.Id("password"));

            //try Xpath instead of Id
            IWebElement passwordField = driver.FindElement(By.XPath("//input[@id='password']"));
            passwordField.SendKeys("secret_sauce");

            // 5. ACTION: Click the Login Button
            //IWebElement loginButton = driver.FindElement(By.Id("login-button"));
            IWebElement loginButton = driver.FindElement(By.XPath("//input[@id='login-button']"));
            loginButton.Click();
            Console.WriteLine("Clicked Login...");

            // 6. WAIT: Pause for 3 seconds to let the page load (Simple method)
            //Thread.Sleep(3000);
            wait.Until(d => d.Url.Contains("inventory"));

            // 7. VERIFY: Check if the login worked by looking at the URL
            string currentUrl = driver.Url;
            if (currentUrl.Contains("inventory"))
            {
                Console.WriteLine("✅ TEST PASSED: We are inside the shop!");
            }
            else
            {
                Console.WriteLine("❌ TEST FAILED: Still on login page.");
            }

            Console.WriteLine("checking price...");

            // 1. Find the Price Element
            // On the website, the price text is inside a class called "inventory_item_price"
            // Since there are many items, this finds the FIRST one it sees.
            //IWebElement priceLabel = driver.FindElement(By.ClassName("inventory_item_price"));

            //try Xpath instead of ClassName
            IWebElement priceLabel = driver.FindElement(By.XPath("//div[@class='inventory_item_price']"));

            // 2. Get the text from that element
            string actualPrice = priceLabel.Text;
            Console.WriteLine("Price found: " + actualPrice);

            // 3. Verify it is correct
            if (actualPrice == "$29.99")
            {
                Console.WriteLine("✅ PRICE CHECK PASSED: Backpack is $29.99");
            }
            else
            {
                Console.WriteLine("❌ PRICE CHECK FAILED: Expected $29.99 but found " + actualPrice);
            }

            Console.WriteLine("Adding to cart...");

            // 1. Click the "Add to Cart" button
            // Note: We use the ID for the "Add" state, not the "Remove" state!
            //IWebElement addToCartBtn = driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack"));

            //try Xpath instead of Id
            IWebElement addToCartBtn = driver.FindElement(By.XPath("//button[@id='add-to-cart-sauce-labs-backpack']"));
            addToCartBtn.Click();
            
            // 2. Wait a moment for the animation
            //Thread.Sleep(1000);
            //wait.Until(d => d.FindElement(By.ClassName("shopping_cart_badge")));
            wait.Until(d => d.FindElement(By.XPath("//span[@class='shopping_cart_badge']")));

            // 3. Verify the Cart Badge "1" appeared
            // The red bubble has the class "shopping_cart_badge"
            try 
            {
                //IWebElement cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge"));
                IWebElement cartBadge = driver.FindElement(By.XPath("//span[@class='shopping_cart_badge']"));
                string itemsInCart = cartBadge.Text;
                
                if (itemsInCart == "1")
                {
                    Console.WriteLine("✅ CART CHECK PASSED: 1 Item in cart!");
                }
                else 
                {
                    Console.WriteLine("❌ CART CHECK FAILED: Expected 1, found " + itemsInCart);
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("❌ CART CHECK FAILED: Cart badge did not appear.");
            }

            Console.WriteLine("Logging out...");

            // 1. Click the Menu
            //driver.FindElement(By.Id("react-burger-menu-btn")).Click();
            driver.FindElement(By.XPath("//button[@id='react-burger-menu-btn']")).Click();

            // 2. Wait for animation (Crucial for sliding menus)
            Thread.Sleep(1000); 

            // 3. Find the Link
            //IWebElement logoutLink = driver.FindElement(By.Id("logout_sidebar_link"));
            IWebElement logoutLink = driver.FindElement(By.XPath("//a[@id='logout_sidebar_link']"));

            // --- THE FIX: FORCE CLICK (JavaScript) ---
            // This bypasses the UI and forces the browser to trigger the click event.
            // It works even if the menu is slightly stuck or moving.
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", logoutLink);

            // 5. Wait for URL to change (Smart Wait)
            try 
            {
                // Wait until we are NO LONGER on the inventory page
                wait.Until(d => !d.Url.Contains("inventory")); 
                Console.WriteLine("✅ LOGOUT SUCCESSFUL: We are back at the login screen.");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("❌ LOGOUT FAILED: URL never changed. Current: " + driver.Url);
            }

            // 8. CLEANUP: Close the browser
            driver.Quit();
        }
    }
}