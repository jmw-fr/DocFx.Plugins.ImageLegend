// <copyright file="ImageLegend.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DocFx.Plugins.ImageLegend
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using HtmlAgilityPack;
    using Microsoft.DocAsCode.Plugins;

    /// <summary>
    /// This processor adds an image legend after the img tag
    /// in the output html.
    /// </summary>
    [Export(nameof(ImageLegend), typeof(IPostProcessor))]
    public class ImageLegend : IPostProcessor
    {
        private readonly string legendTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLegend"/> class.
        /// </summary>
        public ImageLegend()
        {
            legendTemplate = LoadResString("legend.html");
        }

        /// <inheritdoc />
        public ImmutableDictionary<string, object> PrepareMetadata(ImmutableDictionary<string, object> metadata)
        {
            return metadata;
        }

        /// <inheritdoc />
        public Manifest Process(Manifest manifest, string outputFolder)
        {
            DateTime dtStart = DateTime.Now;

            Console.WriteLine("[DocFx.Plugins.ImageLegend] Start processing files.");

            foreach (ManifestItem manifestItem in manifest.Files.Where(f => f.DocumentType == "Conceptual"))
            {
                foreach (OutputFileInfo outputFile in manifestItem.OutputFiles.Values)
                {
                    ProcessFile(Path.Combine(outputFolder, outputFile.RelativePath));
                }
            }

            Console.WriteLine($"[DocFx.Plugins.ImageLegend] End processing in {(DateTime.Now - dtStart).Milliseconds} ms.");

            return manifest;
        }

        /// <summary>
        /// Process and transform an html file.
        /// </summary>
        /// <param name="completeFileName">Complete file name (with path).</param>
        private void ProcessFile(string completeFileName)
        {
            Console.WriteLine($"[DocFx.Plugins.ImageLegend] Processing {completeFileName}.");

            var htmldoc = new HtmlDocument();

            htmldoc.Load(completeFileName);

            var imgNodes = htmldoc.DocumentNode.SelectNodes("//img");

            foreach (var imgNode in imgNodes.Where(img => string.Compare(img.Id, "logo", true) != 0))
            {
                AddLegend(htmldoc, imgNode);
            }

            htmldoc.Save(completeFileName);
        }

        /// <summary>
        /// Adds the legend to the image.
        /// </summary>
        /// <param name="htmldoc">Html document.</param>
        /// <param name="imgNode">Image node.</param>
        private void AddLegend(HtmlDocument htmldoc, HtmlNode imgNode)
        {
            string title = imgNode.GetAttributeValue("title", null);

            if (!string.IsNullOrWhiteSpace(title))
            {
                string legendText = legendTemplate
                                        .Replace("{title}", HttpUtility.HtmlEncode(title))
                                        .Replace("{img}", imgNode.OuterHtml);
                HtmlNode parentNode = imgNode.ParentNode;
                HtmlNode legendNode = HtmlNode.CreateNode(legendText);

                parentNode.ReplaceChild(legendNode, imgNode);
            }
        }

        /// <summary>
        /// Loads the template from the resources.
        /// </summary>
        /// <param name="resourceName">ResourceName</param>
        /// <returns>Template string.</returns>
        private string LoadResString(string resourceName)
        {
            using (var stream = Assembly
                            .GetExecutingAssembly()
                            .GetManifestResourceStream($"DocFx.Plugins.ImageLegend.{resourceName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
