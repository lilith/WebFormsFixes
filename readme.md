# NathanaelJones.WebFormsFixes

A small collection of utilities to make working with Microsoft WebForms a tiny bit more bearable.

[![No Maintenance Intended](http://unmaintained.tech/badge.svg)](http://unmaintained.tech/)

I wrote these as part of an hybrid file-based CMS, and decided to extract them for more generic use.

I have not had the opportunity to test these, and may have introduced typo-style errors while re-extracting 
them from the CMS. I previously published extracted versions along with the two blog articles referenced below, 
but lost them along with much other website data.


Related articles:

http://nathanaeljones.com/152/extending-page-adding-metadata-and-stylesheet-management/
http://nathanaeljones.com/146/referencing-stylesheets-scripts-from-content-pages/

Related MS Connect issues

http://connect.microsoft.com/VisualStudio/feedback/details/273683/contentplaceholder-inside-the-head-element-special-head-parsing-logic-no-longer-applies
http://connect.microsoft.com/VisualStudio/feedback/details/105064/contentplaceholder-control-in-the-head-section-results-in-an-error



While this library is designed to run under .NET 2.0, you could easily make extension methods for LinkManager and MetadataManager, removing the need for PageBase or a similar subclass.

## No dependencies

* System (2.0 or 4.0)
* System.Web (2.0 or 4.0)

## Summary of classes

* ControlUtils - Provides methods for searching control hierarchies by type. Used by ContentPlaceHolderFixes, LinkManager, MetadataManager, and PageBase.
* ContentPlaceHolderFixes - Provides methods for cleaning up errors made by the ASP.NET parser when a contentplaceholder (CPH from here on) is put inside the head section of a Master Page.
* MetadataManager - Manages &lt;meta> tags (controls) for a given Page instance. Not designed for HTTP-EQUIV tags - they are ignored and skipped unless they have a name attribute. Only deals with meta tags within the Head of the page. The page must have a server-side head tag.
* LinkManager - Manages &lt;link> stylehseet tags (controls) for a given Page instance.
* PageBase - Extends Page, adding support for referer tracking across postbacks, alternate 'runtime' master pages (great for design-time CMS support), and integrates the content place holder fixes, MetadataManger, and LinkManager.
* yrl - A mutable class for application-relative paths. Use minimally by PageBase, but easily removable.
* Adapters/HideIDAdapter - When applied with an adapters file, allows any control to disable ID rendering with the HideID="true" attribute.
* Adapters/HideIDAlwaysAdapter - When applied with an adapters file, disables ID rendering for any applied controls. Can be overriden with the HideID="false" attribute.
* Adapters/FixHtmlAnchorAmersandAdapter - Fixes ampersand encoding in the HtmlAnchor class, and supports the HideID attribute (defaults to false).
