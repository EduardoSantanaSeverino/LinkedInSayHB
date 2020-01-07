# LinkedInSayHB Technical Definition
The main method will run and execute the main tasks of the app:

  [STAThread]
        public static void Main(string[] args)
        {

            ValidateParams(args);
           
            using (IWebDriver driver = new ChromeDriver(Environment.CurrentDirectory))
            {
                driver.Navigate().GoToUrl("https://www.linkedin.com/login/");
                driver.Manage().Window.Maximize();
                driver.FindElement(By.Id("username"),timeOutInSeconds).SendKeys(email);
                driver.FindElement(By.Id("password"),timeOutInSeconds).SendKeys(password);
                driver.FindElement(By.CssSelector(".login__form_action_container > button"),timeOutInSeconds).Click();
                driver.Navigate().GoToUrl("https://www.linkedin.com/notifications/");
                
                Task.Delay(timeOutDelay).Wait();
             
                try
                {
                    driver.FindElement(By.XPath("//span[text() = \"See who's celebrating\"]"),timeOutInSeconds).Click();
                }
                catch (Exception e)
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    Task.Delay(timeOutDelay).Wait();
                    driver.FindElement(By.XPath("//span[text() = \"See who's celebrating\"]"),timeOutInSeconds).Click();
                }
             
                Task.Delay(timeOutDelay).Wait();
                var nameList = driver.FindElements(By.CssSelector("article a.nt-card__headline"),timeOutInSeconds);
                var alreadySent = driver.FindElements(By.CssSelector("article div.t-14.t-black--light"),timeOutInSeconds).ToArray().Select(item => item.Text);
                var buttonsToSayHappy = driver.FindElements(By.XPath("//span[text() = 'Say happy birthday']"),timeOutInSeconds);
                
                var c1 = nameList.Count;
                Console.WriteLine("Count Name List: " + c1);
                var c2 = alreadySent.Count();
                Console.WriteLine("Count Already Sended: " + c2);
                var c3 = buttonsToSayHappy.Count;
                Console.WriteLine("Count Buttons to say happy: " + c3);

                var index = 0;
                foreach (var item in nameList.ToArray())
                {
                    string name = "";
                    bool staleElement = true; 
                    while(staleElement){
                        try{
                            name = item.Text;
                            staleElement = false;
                        } catch(StaleElementReferenceException e){
                            staleElement = true;
                        }
                    }
                    
                    name = name.Replace("Wish", "");
                    name = name.Replace("a happy birthday", "");
                    name = name.Replace("(today)", "").Trim();
                    name = name.Replace("(yesterday)", "").Trim();
                    
                    var firsName = name.Substring(0,name.IndexOf(" ", StringComparison.Ordinal));

                    var existInList = alreadySent.Any(l => l.Contains(firsName));

                    if (!existInList)
                    {
                        
                        buttonsToSayHappy[index].Click();
                        
                        driver.FindElement(By.CssSelector("form.msg-form div.msg-form__contenteditable > p"), timeOutInSeconds)
                            .Click();
                        
                        driver.FindElement(By.CssSelector("form.msg-form div.msg-form__contenteditable > p"), timeOutInSeconds)
                            .SendKeys(" " + firsName + (hasCustomMessage ? " " + message.Trim() : " Best wishes!"));
                        
                        driver.FindElement(By.CssSelector("form.msg-form div.msg-form__contenteditable > p"), timeOutInSeconds)
                            .SendKeys(Keys.Enter);

                         Task.Delay(timeOutDelay).Wait();
                        
                        driver.FindElement(SelectorByAttributeValue("data-control-name","overlay.close_conversation_window"), timeOutInSeconds)
                            .Click();
                        
                        index++;
                        
                    }
                    
                    Task.Delay(timeOutDelay).Wait();
                    
                }
                
            }
           
        }
 
