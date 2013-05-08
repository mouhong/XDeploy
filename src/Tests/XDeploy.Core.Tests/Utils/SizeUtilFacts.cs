using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Utils;
using Xunit;

namespace XDeploy.Core.Tests.Utils
{
    public class SizeUtilFacts
    {
        [Fact]
        public void CanGetFriendlyDisplay()
        {
            Assert.Equal("5 B", SizeUtil.GetFriendlyDisplay(5));
            Assert.Equal("1 KB", SizeUtil.GetFriendlyDisplay(1024));
            Assert.Equal("1.5 KB", SizeUtil.GetFriendlyDisplay(1500));
            Assert.Equal("2 MB", SizeUtil.GetFriendlyDisplay(1024 * 1024 * 2));
            Assert.Equal("2.6 MB", SizeUtil.GetFriendlyDisplay(2726297));
            Assert.Equal("1 GB", SizeUtil.GetFriendlyDisplay(1024 * 1024 * 1024));
            Assert.Equal("1.1 GB", SizeUtil.GetFriendlyDisplay(1181116006));
        }
    }
}
