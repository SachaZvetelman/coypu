﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SHDocVw;
using WatiN.Core;
using WatiN.Core.Constraints;
using WatiN.Core.Native.Windows;
using mshtml;

namespace Coypu.Drivers.Watin
{
    public class WatiNDriver : Driver
    {
        private readonly ElementFinder elementFinder;

        private DialogHandler watinDialogHandler;

        static WatiNDriver()
        {
            ElementFactory.RegisterElementType(typeof(Fieldset));
            ElementFactory.RegisterElementType(typeof(Section));
        }

        public WatiNDriver(Browser browser)
        {
            if (browser != Browser.InternetExplorer)
                throw new BrowserNotSupportedException(browser, GetType());

            Settings.AutoMoveMousePointerToTopLeft = false;

            Watin = CreateBrowser();
            elementFinder = new ElementFinder();
        }

        private WatiN.Core.Browser CreateBrowser()
        {
            var browser = new IEWithDialogWaiter();

            AddDialogHandler(browser);

            return browser;
        }

        private void AddDialogHandler(IEWithDialogWaiter browser)
        {
            watinDialogHandler = new DialogHandler();
            browser.AddDialogHandler(watinDialogHandler);
        }

        public void SetBrowser(WatiN.Core.Browser browser)
        {
            Watin.Dispose();
            browser.AddDialogHandler(watinDialogHandler);
            Watin = browser;
        }

        internal WatiN.Core.Browser Watin { get; private set; }

        private static WatiN.Core.Element WatiNElement(Element element)
        {
            return WatiNElement<WatiN.Core.Element>(element);
        }

        private static T WatiNElement<T>(Element element)
            where T : WatiN.Core.Element
        {
            return element.Native as T;
        }

        private Element BuildElement(WatiN.Core.Element element)
        {
            return new WatiNElement(element,Watin);
        }

        private static Element BuildElement(WatiN.Core.Browser browser)
        {
            return new WatiNWindow(browser);
        }

        private WatiN.Core.Browser GetWindowScope(Scope scope)
        {
            var elementScope = ElementFinder.WatiNScope(scope);
            var browserScope = elementScope as WatiN.Core.Browser;

            if (browserScope == null)
            {
                throw new InvalidOperationException("Window level operation called on an element scope");
            }

            return browserScope;
        }

        private static Element BuildElement(Frame frame)
        {
            return new WatiNFrame(frame);
        }

        public object ExecuteScript(string javascript, Scope scope, params object[] args)
        {
            if (args.Length > 0)
                throw new NotSupportedException ("WatiN does not support the passing of arguments to JavaScript.");

            var stripReturn = Regex.Replace(javascript, @"^\s*return ", "");
            var retval = GetWindowScope(scope).Eval(stripReturn);
            Watin.WaitForComplete();
            return retval;
        }

        public void Hover(Element element)
        {
            WatiNElement(element).FireEvent("onmouseover");
        }

        public IEnumerable<Cookie> GetBrowserCookies()
        {
            var ieBrowser = Watin as IE;
            if (ieBrowser == null)
                throw new NotSupportedException("Only supported for Internet Explorer");

            var persistentCookies = GetPersistentCookies(ieBrowser).ToList();
            var documentCookies = GetCookiesFromDocument(ieBrowser);

            var sessionCookies = documentCookies.Except(persistentCookies, new CookieNameEqualityComparer());

            return persistentCookies.Concat(sessionCookies).ToList();
        }

        public void ClearBrowserCookies()
        {
            var ieBrowser = Watin as IE;
            if (ieBrowser == null)
                throw new NotSupportedException("Only supported for Internet Explorer");

            ieBrowser.ClearCookies();
        }

        public IEnumerable<Element> FindWindows(string titleOrName, Scope scope, Options options)
        {
            return new[] { new WatiNWindow(FindWindowHandle(titleOrName, options)) };
        }

        private static Constraint FindWindowHandle(string locator, Options exact)
        {
            Constraint byTitle = Find.ByTitle(locator);
            if (exact.TextPrecision == TextPrecision.Exact)
                byTitle = byTitle | Find.ByTitle(t => t.Contains(locator));

            var by = byTitle | Find.By("hwnd", locator);

            if (Uri.IsWellFormedUriString(locator, UriKind.Absolute))
                by |= Find.ByUrl(locator);

            return by;
        }

        public IEnumerable<Element> FindFrames(string locator, Scope scope, Options options)
        {
            return elementFinder.FindFrames(locator, scope, options).Select(BuildElement);
        }

        public void SendKeys(Element element, string keys)
        {
            foreach (var key in keys)
            {
                WatiNElement<WatiN.Core.Element>(element).KeyPressNoWait(key);
            }
        }

        public void MaximiseWindow(Scope scope)
        {
            GetWindowScope(scope).ShowWindow(NativeMethods.WindowShowStyle.Maximize);
        }

        public void Refresh(Scope scope)
        {
            GetWindowScope(scope).Refresh();
        }

        public void ResizeTo(Size size, Scope scope)
        {
            GetWindowScope(scope).SizeWindow(size.Width, size.Height);
        }

        public void SaveScreenshot(string fileName, Scope scope)
        {
            GetWindowScope(scope).CaptureWebPageToFile(fileName);
        }

        public void GoBack(Scope scope)
        {
            GetWindowScope(scope).Back();
        }

        public void GoForward(Scope scope)
        {
            GetWindowScope(scope).Forward();
        }

        private IEnumerable<Cookie> GetPersistentCookies(IE ieBrowser)
        {
            return ieBrowser.GetCookiesForUrl(Watin.Uri).Cast<Cookie>();
        }

        private IEnumerable<Cookie> GetCookiesFromDocument(IE ieBrowser)
        {
            var document = ((IWebBrowser2)ieBrowser.InternetExplorer).Document as IHTMLDocument2;
            if (document == null)
                throw new InvalidOperationException("Cannot get IE document for cookies");

            return from untrimmedCookie in document.cookie.Split(';')
                   let cookie = untrimmedCookie.Trim()
                   let index = cookie.IndexOf('=')
                   let name = cookie.Substring(0, index)
                   let value = cookie.Substring(index + 1, cookie.Length - index - 1)
                   select new Cookie(name, value);
        }

        public void Click(Element element)
        {
            // If we use Click, then we can get a deadlock if IE is displaying a modal dialog.
            // (Yay COM STA!) Our override of the IE class makes sure the WaitForComplete will
            // check to see if the main window is disabled before continuing with the normal wait
            var nativeElement = WatiNElement(element);
            nativeElement.ClickNoWait();
            nativeElement.WaitForComplete();
        }

        public void Visit(string url, Scope scope)
        {
            GetWindowScope(scope).GoTo(url);
        }

        public void Set(Element element, string value)
        {
            var textField = WatiNElement<TextField>(element);
            if (textField != null)
            {
                textField.TypeText(value);
                return;
            }
            var fileUpload = WatiNElement<FileUpload>(element);
            if (fileUpload != null)
                fileUpload.Set(value);
        }

        public object Native
        {
            get { return Watin; }
        }

        public void Check(Element field)
        {
            WatiNElement<CheckBox>(field).Checked = true;
        }

        public void Uncheck(Element field)
        {
            WatiNElement<CheckBox>(field).Checked = false;
        }

        public void Choose(Element field)
        {
            WatiNElement<RadioButton>(field).Checked = true;
        }

        public bool HasDialog(string withText, Scope scope)
        {
            // TODO: scope is the current window in which to look for a dialog
            return watinDialogHandler.Exists() && watinDialogHandler.Message == withText;
        }

        public Element Window
        {
            get { return BuildElement(Watin); }
        }

        public void AcceptModalDialog(Scope scope)
        {
            // TODO: scope is the current window in which to accept a dialog
            watinDialogHandler.ClickOk();
        }

        public void CancelModalDialog(Scope scope)
        {
            // TODO: scope is the current window in which to accept a dialog
            watinDialogHandler.ClickCancel();
        }

        public IEnumerable<Element> FindAllCss(string cssSelector, Scope scope, Options options, Regex text = null)
        {
            return from element in elementFinder.FindAllCss(cssSelector, scope, options, text)
                   select BuildElement(element);
        }

        public IEnumerable<Element> FindAllXPath(string xpath, Scope scope, Options options)
        {
            return from element in elementFinder.FindAllXPath(xpath, scope, options)
                   select BuildElement(element);
        }

        public Uri Location(Scope scope)
        {
            return GetWindowScope(scope).Uri;
        }

        public string Title(Scope scope)
        {
            return GetWindowScope(scope).Title;
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            watinDialogHandler.Dispose();
            Watin.Dispose();
            Disposed = true;
        }
    }
}
