using Humanizer.Bytes;
using MPC_HC.Domain.Helpers;
using Xunit;

namespace MPC_HC.Test
{
    public class HtmlParserTest
    {
        [Fact]
        public void ConverterTest()
        {
            var htmlStr =
                "    <html lang = \"en\"><head>" +
                "    < meta charset = \"utf-8\">" +
                "    < title > MPC - HC WebServer - Variables </title >" +
                "    <link rel = \"stylesheet\" href=\"default.css\">" +
                "    < link rel = \"icon\" href=\"favicon.ico\">" +
                "    < style type = \"text/css\">" +
                "    :root" +
                "    #header + #content > #left > #rlblock_left" +
                "    {display:none !important;}</style ></head >" +
                "    <body class = \"page-variables\">" +
                "    < !--[if lt IE 8]>" +
                "    <div class = \"browser-warning\"><strong>Warning!</strong> You are using an <strong>outdated</strong> browser." +
                "Please < a href = \"http://browsehappy.com/\">upgrade your browser</a> to improve your experience.</div>" +
                "    < ![endif]-->" +
                "    <p id=\"file\">Gravity.Falls.S01E06.Dipper.vs.Manliness.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv</p>" +
                "    <p id=\"filepatharg\">D:%5cDownloads%5cTorrentDay%5cDownloads%5cGravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ%5cGravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ%5cGravity.Falls.S01E06.Dipper.vs.Manliness.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv</p>" +
                "    <p id=\"filepath\">D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01E06.Dipper.vs.Manliness.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv</p>" +
                "    <p id=\"filedirarg\">D:%5cDownloads%5cTorrentDay%5cDownloads%5cGravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ%5cGravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ</p>" +
                "    <p id=\"filedir\">D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ</p>" +
                "    <p id=\"state\">2</p>" +
                "    <p id=\"statestring\">Playing</p>" +
                "    <p id=\"position\">77149</p>" +
                "    <p id=\"positionstring\">00:01:17</p>" +
                "    <p id=\"duration\">1358858</p>" +
                "    <p id=\"durationstring\">00:22:39</p>" +
                "    <p id=\"volumelevel\">100</p>" +
                "    <p id=\"muted\">1</p>" +
                "    <p id=\"playbackrate\">1</p>" +
                "    <p id=\"size\">532 MB</p>" +
                "    <p id=\"reloadtime\">0</p>" +
                "    <p id=\"version\">1.7.11.0</p>" +
                "    < br ><hr ></body ></html >";

            var info = HtmlParserHelper.ParseHtmlToInfo(htmlStr);
            Assert.Equal("Gravity.Falls.S01E06.Dipper.vs.Manliness.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv", info.FileName);
            Assert.Equal("D:%5cDownloads%5cTorrentDay%5cDownloads%5cGravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ%5cGravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ%5cGravity.Falls.S01E06.Dipper.vs.Manliness.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv", info.FilePathArg);
            Assert.Equal("D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01E06.Dipper.vs.Manliness.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv", info.FilePath);
            Assert.Equal("D:%5cDownloads%5cTorrentDay%5cDownloads%5cGravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ%5cGravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ", info.FileDirArg);
            Assert.Equal("D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ", info.FileDir);
            Assert.Equal("Playing", info.StateString);
            Assert.Equal(State.Playing, info.State);
            Assert.Equal(77149, info.PositionMillisec);
            Assert.Equal(ByteSize.Parse("532 MB"), info.Size);
            Assert.Equal(true, info.Muted);
            Assert.Equal(100, info.VolumeLevel);
            
        }
    }
}