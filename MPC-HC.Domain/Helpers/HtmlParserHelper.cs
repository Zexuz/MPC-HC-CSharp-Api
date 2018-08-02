using System;
using HtmlAgilityPack;

namespace MPC_HC.Domain.Helpers
{
    public static class HtmlParserHelper
    {
        public static Info ParseHtmlToInfo(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            htmlDoc.LoadHtml(htmlDoc.DocumentNode.OuterHtml); //yes this is needed....
            var info = new Info();
            info.FileName = htmlDoc.GetElementbyId("file").InnerText;
            info.FilePathArg = htmlDoc.GetElementbyId("filepatharg").InnerText;
            info.FilePath = htmlDoc.GetElementbyId("filepath").InnerText;
            info.FileDirArg = htmlDoc.GetElementbyId("filedirarg").InnerText;
            info.FileDir = htmlDoc.GetElementbyId("filedir").InnerText;
            info.State = Info.IntToState(Convert.ToInt32(htmlDoc.GetElementbyId("state").InnerText));
            info.StateString = htmlDoc.GetElementbyId("statestring").InnerText;
            info.Position = TimeSpan.FromMilliseconds(Convert.ToDouble(htmlDoc.GetElementbyId("position").InnerText));
            info.PositionMillisec = Convert.ToInt64(htmlDoc.GetElementbyId("position").InnerText);
            info.Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(htmlDoc.GetElementbyId("duration").InnerText));
            info.DurationMillisec = Convert.ToInt64(htmlDoc.GetElementbyId("duration").InnerText);
            info.VolumeLevel = Convert.ToInt32(htmlDoc.GetElementbyId("volumelevel").InnerText);
            info.Muted = Convert.ToBoolean(Convert.ToInt16(htmlDoc.GetElementbyId("muted").InnerText));
            info.PlaybackRate = Convert.ToDouble(htmlDoc.GetElementbyId("playbackrate").InnerText);
            info.SizeString = htmlDoc.GetElementbyId("size").InnerText;
            info.ReloadTime = Convert.ToInt32(htmlDoc.GetElementbyId("reloadtime").InnerText);
            info.Version = htmlDoc.GetElementbyId("version").InnerText;

            return info;
        }
    }
}