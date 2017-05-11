using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class HtmlHelper
    {
        private readonly RazorPage _page;

        public HtmlHelper([NotNull] RazorPage page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            _page = page;
        }
         
        public NonEscapedString RenderPartial(RazorPage partialPage)
        {
            partialPage.Assign(_page);
            return new NonEscapedString(partialPage.ToString());
        }

        public NonEscapedString Raw(string value)
        {
            return new NonEscapedString(value);
        }

        public NonEscapedString JobId(string jobId, bool shorten = true)
        {
            Guid guid;
            return new NonEscapedString(Guid.TryParse(jobId, out guid)
                ? (shorten ? jobId.Substring(0, 8) + "…" : jobId)
                : $"#{jobId}");
        }

       

 
        public NonEscapedString JobIdLink(string jobId)
        {
            return Raw($"<a href=\"{_page.Url.JobDetails(jobId)}\">{JobId(jobId)}</a>");
        }

       
        public string ToHumanDuration(TimeSpan? duration, bool displaySign = true)
        {
            if (duration == null) return null;

            var builder = new StringBuilder();
            if (displaySign)
            {
                builder.Append(duration.Value.TotalMilliseconds < 0 ? "-" : "+");
            }

            duration = duration.Value.Duration();

            if (duration.Value.Days > 0)
            {
                builder.Append($"{duration.Value.Days}d ");
            }

            if (duration.Value.Hours > 0)
            {
                builder.Append($"{duration.Value.Hours}h ");
            }

            if (duration.Value.Minutes > 0)
            {
                builder.Append($"{duration.Value.Minutes}m ");
            }

            if (duration.Value.TotalHours < 1)
            {
                if (duration.Value.Seconds > 0)
                {
                    builder.Append(duration.Value.Seconds);
                    if (duration.Value.Milliseconds > 0)
                    {
                        builder.Append($".{duration.Value.Milliseconds.ToString().PadLeft(3, '0')}");
                    }

                    builder.Append("s ");
                }
                else
                {
                    if (duration.Value.Milliseconds > 0)
                    {
                        builder.Append($"{duration.Value.Milliseconds}ms ");
                    }
                }
            }

            if (builder.Length <= 1)
            {
                builder.Append(" <1ms ");
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        public string FormatProperties(IDictionary<string, string> properties)
        {
            return String.Join(", ", properties.Select(x => $"{x.Key}: \"{x.Value}\""));
        }

     

        public NonEscapedString ServerId(string serverId)
        {
            var parts = serverId.Split(':');
            var shortenedId = parts.Length > 1
                ? String.Join(":", parts.Take(parts.Length - 1))
                : serverId;

            return new NonEscapedString(
                $"<span class=\"labe label-defult text-uppercase\" title=\"{serverId}\">{shortenedId}</span>");
        }

        private static readonly StackTraceHtmlFragments StackTraceHtmlFragments = new StackTraceHtmlFragments
        {
            BeforeFrame = "<span class='st-frame'>",
            AfterFrame = "</span>",
            BeforeType = "<span class='st-type'>",
            AfterType = "</span>",
            BeforeMethod = "<span class='st-method'>",
            AfterMethod = "</span>",
            BeforeParameters = "<span class='st-params'>",
            AfterParameters = "</span>",
            BeforeParameterType = "<span class='st-param'><span class='st-param-type'>",
            AfterParameterType = "</span>",
            BeforeParameterName = "<span class='st-param-name'>",
            AfterParameterName = "</span></span>",
            BeforeFile = "<span class='st-file'>",
            AfterFile = "</span>",
            BeforeLine = "<span class='st-line'>",
            AfterLine = "</span>",
        };

        public NonEscapedString StackTrace(string stackTrace)
        {
            try
            {
                return new NonEscapedString(StackTraceFormatter.FormatHtml(stackTrace, StackTraceHtmlFragments));
            }
            catch (RegexMatchTimeoutException)
            {
                return new NonEscapedString(HtmlEncode(stackTrace));
            }
        }

        public string HtmlEncode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }

        
    }

}
