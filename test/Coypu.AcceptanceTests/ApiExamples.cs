﻿//using System;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Threading;
//using Coypu.Drivers;
//using Coypu.Drivers.Selenium;
//using Xunit;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium.Remote;

//namespace Coypu.AcceptanceTests
//{
//    /// <summary>
//    /// Simple examples for each API method - to show usage and check everything is wired up properly
//    /// </summary>
//    public class Examples : WaitAndRetryExamples
//    {
//        [Fact]
//        public void AcceptModalDialog_example()
//        {
//            browser.ClickLink("Trigger an alert");
//            Assert.True(browser.HasDialog("You have triggered an alert and this is the text."));

//            browser.AcceptModalDialog();
//            Assert.True(browser.HasNoDialog("You have triggered an alert and this is the text."));
//        }

//        [Fact]
//        public void CancelModalDialog_example()
//        {
//            browser.ClickLink("Trigger a confirm");
//            browser.CancelModalDialog();
//            browser.FindLink("Trigger a confirm - cancelled").Now();
//        }

//        [Fact]
//        public void ModalDialog_while_multiple_windows_are_open()
//        {
//            browser.ClickLink("Open pop up window");
//            browser.ClickLink("Trigger a confirm");
//            // browser.ClickLink("Trigger a confirm"); IE driver needs to click twice here - no idea why yet

//            browser.CancelModalDialog();
//            browser.FindLink("Trigger a confirm - cancelled").Now();
//        }

//        [Fact]
//        public void Check_example()
//        {
//            browser.Check("uncheckedBox");
//            Assert.True(browser.FindField("uncheckedBox").Selected);
//        }

//        [Fact]
//        public void Uncheck_example()
//        {
//            browser.Uncheck("checkedBox");
//            Assert.False(browser.Query(() => browser.FindField("checkedBox").Selected, false));
//        }

//        [Fact]
//        public void Can_find_checkbox_and_check_it()
//        {
//            var checkbox = browser.FindCss("#uncheckedBox");
//            checkbox.Check();
//            Assert.True(browser.FindField("uncheckedBox").Selected);
//        }

//        [Fact]
//        public void Can_find_checkbox_and_uncheck_it()
//        {
//            var checkbox = browser.FindCss("#checkedBox");
//            checkbox.Uncheck();
//            Assert.False(browser.Query(() => browser.FindField("checkedBox").Selected, false));
//        }

//        [Fact]
//        public void Choose_example()
//        {
//            browser.Choose("chooseRadio1");

//            Assert.True(browser.FindField("chooseRadio1").Selected);

//            browser.Choose("chooseRadio2");

//            Assert.True(browser.FindField("chooseRadio2").Selected);
//            Assert.False(browser.FindField("chooseRadio1").Selected);
//        }

//        [Fact]
//        public void Click_example()
//        {
//            var element = browser.FindButton("clickMeTest");
//            Assert.Equal("Click me", browser.FindButton("clickMeTest").Value);

//            element.Click();
//            Assert.Equal("Click me - clicked", browser.FindButton("clickMeTest").Value);
//        }

//        [Fact]
//        public void ClickButton_example()
//        {
//            browser.ClickButton("clickMeTest");
//            Assert.Equal("Click me - clicked", browser.FindButton("clickMeTest").Value);
//        }

//        [Fact]
//        public void ClickLink_example()
//        {
//            browser.ClickLink("Trigger a confirm");
//            browser.CancelModalDialog();
//        }

//        [Fact]
//        public void ExecuteScript_example()
//        {
//            ReloadTestPage();
//            Assert.That(browser.ExecuteScript("return document.getElementById('firstButtonId').innerHTML;"),
//                        Is.EqualTo("first button"));
//        }

//        [Fact]
//        public void ExecuteScriptWithArgs_example()
//        {
//            ReloadTestPage();
//            Assert.That(browser.ExecuteScript("return arguments[0].innerHTML;", browser.FindId("firstButtonId")),
//                        Is.EqualTo("first button"));
//        }

//        [Fact]
//        public void FillInWith_example()
//        {
//            browser.FillIn("scope2ContainerLabeledTextInputFieldId").With("New text input value");
//            Assert.That(browser.FindField("scope2ContainerLabeledTextInputFieldId").Value,
//                        Is.EqualTo("New text input value"));
//        }

//        [Fact]
//        public void SendKeys_example()
//        {
//            browser.FindField("containerLabeledTextInputFieldId").SendKeys(" - send these keys");
//            Assert.That(browser.FindField("containerLabeledTextInputFieldId").Value,
//                        Is.EqualTo("text input field two val - send these keys"));
//        }

//        [Fact]
//        public void FillInWith_element_example()
//        {
//            browser.FindField("scope2ContainerLabeledTextInputFieldId").FillInWith("New text input value");
//            Assert.That(browser.FindField("scope2ContainerLabeledTextInputFieldId").Value,
//                        Is.EqualTo("New text input value"));
//        }

//        [Fact]
//        public void SelectFrom_element_example()
//        {
//            var field = browser.FindField("containerLabeledSelectFieldId");
//            Assert.That(field.SelectedOption, Is.EqualTo("select two option one"));

//            field.SelectOption("select two option two");

//            field = browser.FindField("containerLabeledSelectFieldId");
//            Assert.That(field.SelectedOption, Is.EqualTo("select two option two"));
//        }

//        [Fact]
//        public void FindAllCss_example()
//        {
//            ReloadTestPage();

//            const string shouldFind = "#inspectingContent ul#cssTest li";
//            var all = browser.FindAllCss(shouldFind).ToList();
//            Assert.That(all.Count(), Is.EqualTo(3));
//            Assert.That(all.ElementAt(1).Text, Is.EqualTo("two"));
//            Assert.That(all.ElementAt(2).Text, Is.EqualTo("Me! Pick me!"));
//        }

//        [Fact]
//        public void FindAllXPath_example()
//        {
//            ReloadTestPage();

//            const string shouldFind = "//*[@id='inspectingContent']//ul[@id='cssTest']/li";
//            var all = browser.FindAllXPath(shouldFind).ToArray();
//            Assert.That(all.Count(), Is.EqualTo(3));
//            Assert.That(all.ElementAt(1).Text, Is.EqualTo("two"));
//            Assert.That(all.ElementAt(2).Text, Is.EqualTo("Me! Pick me!"));
//        }

//        [Fact]
//        public void FindButton_example()
//        {
//            Assert.That(browser.FindButton("Click me").Id, Is.EqualTo("clickMeTest"));
//        }

//        [Fact]
//        public void DisabledButton_example()
//        {
//            Assert.That(browser.FindButton("Disabled button").Disabled, Is.True, "Expected button to be disabled");
//            Assert.That(browser.FindButton("Click me").Disabled, Is.False, "Expected button to be enabled");
//        }

//        [Fact]
//        public void FindCss_example()
//        {
//            var first = browser.FindCss("#inspectingContent ul#cssTest li", Options.First);
//            Assert.That(first.Text, Is.EqualTo("one"));
//        }

//        [Fact]
//        public void FindCss_with_text_example()
//        {
//            var two = browser.FindCss("#inspectingContent ul#cssTest li", text: "two");
//            Assert.That(two.Text, Is.EqualTo("two"));
//        }

//        [Fact]
//        public void FindCss_with_text_matching()
//        {
//            var two = browser.FindCss("#inspectingContent ul#cssTest li", text: new Regex("wo"));
//            Assert.That(two.Text, Is.EqualTo("two"));
//        }

//        [Fact]
//        public void FindXPath_example()
//        {
//            var first = browser.FindXPath("//*[@id='inspectingContent']//ul[@id='cssTest']/li", Options.First);
//            Assert.That(first.Text, Is.EqualTo("one"));
//        }

//        [Fact]
//        public void FindXPath_with_text_example()
//        {
//            var two = browser.FindXPath("//*[@id='inspectingContent']//ul[@id='cssTest']/li", text: "two");
//            Assert.That(two.Text, Is.EqualTo("two"));
//        }

//        [Fact]
//        public void FindField_examples()
//        {
//            Assert.That(browser.FindField("text input field linked by for", Options.Exact).Id, Is.EqualTo("forLabeledTextInputFieldId"));
//            Assert.That(browser.FindField("checkbox field in a label container").Id,
//                        Is.EqualTo("containerLabeledCheckboxFieldId"));
//            Assert.That(browser.FindField("containerLabeledSelectFieldId").Name,
//                        Is.EqualTo("containerLabeledSelectFieldName"));
//            Assert.That(browser.FindField("containerLabeledPasswordFieldName").Id,
//                        Is.EqualTo("containerLabeledPasswordFieldId"));
//        }

//        [Fact]
//        public void FindFieldset_example()
//        {
//            Assert.That(browser.FindFieldset("Scope 1").Id, Is.EqualTo("fieldsetScope1"));
//        }

//        [Fact]
//        public void FindId_example()
//        {
//            Assert.That(browser.FindId("containerLabeledSelectFieldId").Name,
//                        Is.EqualTo("containerLabeledSelectFieldName"));
//        }

//        [Fact]
//        public void FindIdEndingWith_example()
//        {
//            Assert.That(browser.FindIdEndingWith("aspWebFormsContainerLabeledFileFieldId").Id,
//                            Is.EqualTo("_ctrl01_ctrl02_aspWebFormsContainerLabeledFileFieldId"));
//        }

//        [Fact]
//        public void FindLink_example()
//        {
//            Assert.That(browser.FindLink("Trigger an alert").Id, Is.EqualTo("alertTriggerLink"));
//        }

//        [Fact]
//        public void FindSection_example()
//        {
//            Assert.That(browser.FindSection("Inspecting Content").Id, Is.EqualTo("inspectingContent"));
//            Assert.That(browser.FindSection("Div Section Two h2 with link").Id, Is.EqualTo("divSectionTwoWithLink"));
//        }

//        [Fact]
//        public void SelectFrom_example()
//        {
//            var textField = browser.FindField("containerLabeledSelectFieldId");
//            Assert.That(textField.SelectedOption, Is.EqualTo("select two option one"));

//            browser.Select("select2value2").From("containerLabeledSelectFieldId");

//            textField = browser.FindField("containerLabeledSelectFieldId");
//            Assert.That(textField.SelectedOption, Is.EqualTo("select two option two"));
//        }

//        [Fact]
//        public void HasContent_example()
//        {
//            Assert.That(browser, Shows.Content("This is what we are looking for"));
//            Assert.That(browser.HasContent("This is not in the page"), Is.False);

//            Assert.Throws<AssertionException>(() => Assert.That(browser, Shows.Content("This is not in the page")));
//        }

//        [Fact]
//        public void HasContentMatching_example()
//        {
//            Assert.That(browser, Shows.Content(new Regex(@"This.is.what.we.are.looking.for")));
//            Assert.That(browser.HasContentMatch(new Regex(@"This.is.not.in.the.page")), Is.False);

//            Assert.Throws<AssertionException>(() => Assert.That(browser, Shows.Content(new Regex(@"This.is.not.in.the.page"))));
//        }

//        [Fact]
//        public void HasNoContent_example()
//        {
//            browser.ExecuteScript(
//                "document.body.innerHTML = '<div id=\"no-such-element\">This is not in the page</div>'");
//            Assert.That(browser, Shows.No.Content("This is not in the page"));

//            ReloadTestPage();
//            Assert.That(browser.HasNoContent("This is what we are looking for"), Is.False);

//            Assert.Throws<AssertionException>(() => Assert.That(browser, Shows.No.Content("This is what we are looking for")));
//        }

//        [Fact]
//        public void HasContentMatch_example()
//        {
//            Assert.True(browser.HasContentMatch(new Regex("This is what (we are|I am) looking for")));
//            Assert.False(browser.HasContentMatch(new Regex("This is ?n[o|']t in the page")));
//        }

//        [Fact]
//        public void HasNoContentMatch_example()
//        {
//            browser.ExecuteScript(
//                "document.body.innerHTML = '<div id=\"no-such-element\">This is not in the page</div>'");
//            Assert.True(browser.HasNoContentMatch(new Regex("This is ?n[o|']t in the page")));

//            ReloadTestPage();
//            Assert.False(browser.HasNoContentMatch(new Regex("This is what (we are|I am) looking for")));
//        }

//        [Fact]
//        public void HasValue_example()
//        {
//            var field = browser.FindField("find-this-field");
//            Assert.That(field, Shows.Value("This value is what we are looking for"));
//            Assert.False(field.HasValue("This is not the value"));
//        }

//        [Fact]
//        public void Attributes_on_stale_scope_example()
//        {
//            var field = browser.FindField("find-this-field");

//            Assert.That(field.Value, Is.EqualTo("This value is what we are looking for"));

//            ReloadTestPage();

//            Assert.That(field.Value, Is.EqualTo("This value is what we are looking for"));
//            Assert.That(field.Id, Is.EqualTo("find-this-field"));
//            Assert.That(field["id"], Is.EqualTo("find-this-field"));

//        }

//        [Fact]
//        public void HasNoValue_example()
//        {
//            var field = browser.FindField("find-this-field");
//            Assert.That(field, Shows.No.Value("This is not the value"));
//            Assert.False(field.HasNoValue("This value is what we are looking for"));
//        }

//        [Fact]
//        public void ShowsAllCssInOrder_example()
//        {
//            Assert.That(browser, Shows.AllCssInOrder("#inspectingContent ul li", new[] { "Some", "text", "in", "a", "list","one","two","Me! Pick me!"}));
//            Assert.Throws<AssertionException>(() => Assert.That(browser, Shows.AllCssInOrder("#inspectingContent ul li", new[] { "Some", "text", "in", "a", "list","two", "one","Me! Pick me!"})));
//        }


//        [Fact]
//        public void ShowsCssContaining_example()
//        {
//            Assert.That(browser, Shows.CssContaining("#inspectingContent ul li", "Some", "text","in","a","list"));
//            Assert.Throws<AssertionException>(() => Assert.That(browser, Shows.CssContaining("#inspectingContent ul li", "missing","from","a","list")));
//        }


//        [Fact]
//        public void ShowsContentContaining_example()
//        {
//            Assert.That(browser, Shows.ContentContaining("Some", "text", "in", "a", "list"));
//            Assert.Throws<AssertionException>(() => Assert.That(browser, Shows.ContentContaining("this is not in the page", "in", "a", "list")));
//        }

//        [Fact]
//        public void Hover_example()
//        {
//            Assert.That(browser.FindId("hoverOnMeTest").Text, Is.EqualTo("Hover on me"));
//            browser.FindId("hoverOnMeTest").Hover();
//            Assert.That(browser.FindId("hoverOnMeTest").Text, Is.EqualTo("Hover on me - hovered"));
//        }

//        [Fact]
//        public void Native_example()
//        {
//            var button = (IWebElement) browser.FindButton("clickMeTest").Native;
//            button.Click();
//            Assert.That(browser.FindButton("clickMeTest").Value, Is.EqualTo("Click me - clicked"));
//        }

//        [Fact]
//        public void Title_example()
//        {
//            Assert.That(browser.Title, Is.EqualTo("Coypu interaction tests page"));
//        }

//        [Fact]
//        public void Within_example()
//        {
//            const string locatorThatAppearsInMultipleScopes = "scoped text input field linked by for";

//            var expectingScope1 = browser.FindId("scope1").FindField(locatorThatAppearsInMultipleScopes);
//            var expectingScope2 = browser.FindId("scope2").FindField(locatorThatAppearsInMultipleScopes);

//            Assert.That(expectingScope1.Id, Is.EqualTo("scope1TextInputFieldId"));
//            Assert.That(expectingScope2.Id, Is.EqualTo("scope2TextInputFieldId"));
//        }

//        [Fact]
//        public void WithinFieldset_example()
//        {
//            const string locatorThatAppearsInMultipleScopes = "scoped text input field linked by for";

//            var expectingScope1 = browser.FindFieldset("Scope 1")
//                                         .FindField(locatorThatAppearsInMultipleScopes);

//            var expectingScope2 = browser.FindFieldset("Scope 2")
//                                         .FindField(locatorThatAppearsInMultipleScopes);

//            Assert.That(expectingScope1.Id, Is.EqualTo("scope1TextInputFieldId"));
//            Assert.That(expectingScope2.Id, Is.EqualTo("scope2TextInputFieldId"));
//        }

//        [Fact]
//        public void WithinSection_example()
//        {
//            const string selectorThatAppearsInMultipleScopes = "h2";

//            var expectingScope1 = browser.FindSection("Section One h1").FindCss(selectorThatAppearsInMultipleScopes);
//            var expectingScope2 = browser.FindSection("Div Section Two h1").FindCss(selectorThatAppearsInMultipleScopes);

//            Assert.That(expectingScope1.Text, Is.EqualTo("Section One h2"));
//            Assert.That(expectingScope2.Text, Is.EqualTo("Div Section Two h2"));
//        }

//        [Fact]
//        public void TryUntil_example()
//        {
//            var tryThisButton = browser.FindButton("try this");
//            Assert.That(tryThisButton.Exists());
//            browser.TryUntil(() => tryThisButton.Click(),
//                             () => browser.HasContent("try until 5"),
//                             TimeSpan.FromMilliseconds(50),
//                             new Options {Timeout = TimeSpan.FromMilliseconds(10000)});
//        }

//        [Fact]
//        public void WithinIFrame_example()
//        {
//            const string selectorThatAppearsInMultipleScopes = "scoped button";

//            var expectingScope1 = browser.FindFrame("iframe1").FindButton(selectorThatAppearsInMultipleScopes);
//            var expectingScope2 = browser.FindCss("#iframe2").FindButton(selectorThatAppearsInMultipleScopes);

//            Assert.That(expectingScope1.Id, Is.EqualTo("iframe1ButtonId"));
//            Assert.That(expectingScope2.Id, Is.EqualTo("iframe2ButtonId"));
//        }

//        [Fact]
//        public void WithinIFrame_FoundByCss_example()
//        {
//            const string selectorThatAppearsInMultipleScopes = "scoped button";

//            var expectingScope1 = browser.FindCss("iframe#iframe1").FindButton(selectorThatAppearsInMultipleScopes);
//            var expectingScope2 = browser.FindCss("iframe#iframe2").FindButton(selectorThatAppearsInMultipleScopes);

//            Assert.That(expectingScope1.Id, Is.EqualTo("iframe1ButtonId"));
//            Assert.That(expectingScope2.Id, Is.EqualTo("iframe2ButtonId"));
//        }

//        [Fact]
//        public void WithinFrame_example()
//        {
//            browser.Visit(TestPageLocation("frameset.htm"));

//            const string selectorThatAppearsInMultipleScopes = "scoped button";

//            var expectingScope1 = browser.FindFrame("frame1").FindButton(selectorThatAppearsInMultipleScopes);
//            var expectingScope2 = browser.FindFrame("frame2").FindButton(selectorThatAppearsInMultipleScopes);

//            Assert.That(expectingScope1.Id, Is.EqualTo("frame1ButtonId"));
//            Assert.That(expectingScope2.Id, Is.EqualTo("frame2ButtonId"));
//        }

//        [Fact]
//        public void Multiple_interactions_within_iframe_example()
//        {
//            var iframe = browser.FindFrame("I am iframe one");
//            iframe.FillIn("text input in iframe").With("filled in");
//            Assert.Equal("filled in", iframe.FindField("text input in iframe").Value);
//        }

//        [Fact]
//        public void FillIn_file_example()
//        {
//            const string someLocalFile = @"local.file";
//            try
//            {
//                var directoryInfo = new DirectoryInfo(".");
//                var fullPath = Path.Combine(directoryInfo.FullName, someLocalFile);
//                using (File.Create(fullPath))
//                {
//                }

//                browser.FillIn("forLabeledFileFieldId").With(fullPath);

//                var findAgain = browser.FindField("forLabeledFileFieldId");
//                Assert.That(findAgain.Value, Is.StringEnding(someLocalFile));
//            }
//            finally
//            {
//                File.Delete(someLocalFile);
//            }
//        }

//        [Fact]
//        public void ConsideringInvisibleElements()
//        {
//            browser.FindButton("firstInvisibleInputId", new Options {ConsiderInvisibleElements = true}).Now();
//        }

//        [Fact]
//        public void ConsideringOnlyVisibleElements()
//        {
//            Assert.Throws<MissingHtmlException>(() => browser.FindButton("firstInvisibleInputId").Now());
//        }

//        [Fact]
//        public void WindowScoping_example()
//        {
//            var mainWindow = browser;
//            Assert.That(mainWindow.FindButton("scoped button", Options.First).Id, Is.EqualTo("scope1ButtonId"));
            
//            mainWindow.ExecuteScript("setTimeout(function() {document.getElementById(\"openPopupLink\").click();}), 3000");

//            var popUp = mainWindow.FindWindow("Pop Up Window");
//            Assert.That(popUp.FindButton("scoped button").Id, Is.EqualTo("popUpButtonId"));

//            // And back
//            Assert.That(mainWindow.FindButton("scoped button", Options.First).Id, Is.EqualTo("scope1ButtonId"));
//        }

//        [Fact]
//        public void MaximiseWindow()
//        {
//            var availWidth = browser.ExecuteScript("return window.screen.availWidth;");
//            var initalWidth = GetOuterWidth();

//            Assert.True(double.Parse(initalWidth.ToString()) < double.Parse(availWidth.ToString()));

//            browser.MaximiseWindow();

//            Assert.True(double.Parse(GetOuterWidth().ToString()) >= double.Parse(availWidth.ToString()));
//        }

//        [Fact]
//        public void ResizeWindow()
//        {
//            var initalWidth = GetOuterWidth();
//            var initialHeight = GetOuterHeight();

//            Assert.NotEqual(500, initalWidth);
//            Assert.NotEqual(600, initialHeight);

//            browser.ResizeTo(500, 600);

//            Assert.Equal(500, GetOuterWidth());
//            Assert.Equal(600, GetOuterHeight());
//        }

//        private object GetOuterHeight()
//        {
//            return browser.ExecuteScript("return window.outerHeight;");
//        }

//        private object GetOuterWidth()
//        {
//            return browser.ExecuteScript("return window.outerWidth;");
//        }

//        [Fact]
//        public void RefreshingWindow()
//        {
//             var tickBeforeRefresh = (Int64) browser.ExecuteScript("return window.SpecData.CurrentTick;");
 
//             browser.Refresh();
 
//             var tickAfterRefresh = (Int64) browser.ExecuteScript("return window.SpecData.CurrentTick;");
 
//             Assert.True((tickAfterRefresh - tickBeforeRefresh) > 0);
//        }

//        [Fact]
//        public void CustomProfile()
//        {
//            var configuration = new SessionConfiguration {Driver = typeof (CustomFirefoxProfileSeleniumWebDriver)};

//            using (var custom = new BrowserSession(configuration))
//            {
//                custom.Visit("https://www.relishapp.com/");
//                Assert.Equal(0, custom.ExecuteScript("return 0;"));
//            }
//        }

//        public class CustomFirefoxProfileSeleniumWebDriver : SeleniumWebDriver
//        {
//            public CustomFirefoxProfileSeleniumWebDriver(Drivers.Browser browser)
//                : base(CustomProfile(), browser)
//            {
//            }

//            private static RemoteWebDriver CustomProfile()
//            {
//                var yourCustomProfile = new FirefoxProfile();
//                return new FirefoxDriver(yourCustomProfile);
//            }
//        }

//        [Theory]
//        [InlineData("Windows 7", "firefox", "25")]
//        [InlineData("Windows XP", "internet explorer", "6")]
//        public void CustomBrowserSession(string platform, string browserName, string version)
//        {

//            var desiredCapabilites = new DesiredCapabilities(browserName, version, Platform.CurrentPlatform);
//            desiredCapabilites.SetCapability("platform", platform);
//            desiredCapabilites.SetCapability("username", "appiumci");
//            desiredCapabilites.SetCapability("accessKey", "af4fbd21-6aee-4a01-857f-c7ffba2f0a50");
//            desiredCapabilites.SetCapability("name", TestContext.CurrentContext.Test.Name);

//            Driver driver = new CustomDriver(Browser.Parse(browserName), desiredCapabilites);

//            using (var custom = new BrowserSession(driver))
//            {
//                custom.Visit("https://saucelabs.com/test/guinea-pig");
//                Assert.Equal(0, custom.ExecuteScript("return 0;"));
//            }
//        }

//        public class CustomDriver : SeleniumWebDriver
//        {
//            public CustomDriver(Browser browser, ICapabilities capabilities)
//                : base(CustomWebDriver(capabilities), browser) { }

//            private static RemoteWebDriver CustomWebDriver(ICapabilities capabilities)
//            {
//                var remoteAppHost = new Uri("http://ondemand.saucelabs.com:80/wd/hub");
//                return new RemoteWebDriver(remoteAppHost, capabilities);
//            }
//        }
//    }
//}