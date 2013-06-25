﻿using System;
using ArchiCop.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiCop.InfoData
{
    [TestClass]
    public class LoadEngineInfoTests
    {
        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void ThrowsCorrectExceptionWhenCreatingLoadEngineWithWrongTypeName()
        {
            //
            var loadEngineInfo = new LoadEngineInfo("ArchiCop.InfoData.IDontExist,ArchiCopCore.Tests")
                {
                    Arg1 = "",
                    Arg2 = ""
                };

            //
            var loadEngine = (ILoadEngine) loadEngineInfo.CreateLoadEngine();

            //
            Assert.IsNotNull(loadEngine);
        }

        [TestMethod]
        [ExpectedException(typeof (MissingMethodException))]
        public void ThrowsCorrectExceptionWhenCreatingLoadEngineWithWrongArgs()
        {
            //
            var loadEngineInfo = new LoadEngineInfo("ArchiCop.InfoData.LoadEngine0,ArchiCopCore.Tests")
                {
                    Arg1 = "one",
                    Arg2 = "two"
                };

            //
            var loadEngine = (ILoadEngine) loadEngineInfo.CreateLoadEngine();

            //
            Assert.IsNotNull(loadEngine);
        }

        [TestMethod]
        public void CanCreateLoadEngine0()
        {
            //
            var loadEngineInfo = new LoadEngineInfo("ArchiCop.InfoData.LoadEngine0,ArchiCopCore.Tests")
                {
                    Arg1 = "",
                    Arg2 = ""
                };

            //
            var loadEngine = (ILoadEngine) loadEngineInfo.CreateLoadEngine();

            //
            Assert.IsNotNull(loadEngine);
        }

        [TestMethod]
        public void CanCreateLoadEngine1()
        {
            //
            var loadEngineInfo = new LoadEngineInfo("ArchiCop.InfoData.LoadEngine1,ArchiCopCore.Tests")
                {
                    Arg1 = "something",
                    Arg2 = ""
                };

            //
            var loadEngine = (ILoadEngine) loadEngineInfo.CreateLoadEngine();

            //
            Assert.IsNotNull(loadEngine);
        }

        [TestMethod]
        public void CanCreateLoadEngine2()
        {
            //
            var loadEngineInfo = new LoadEngineInfo("ArchiCop.InfoData.LoadEngine2,ArchiCopCore.Tests")
                {
                    Arg1 = "something",
                    Arg2 = "something"
                };

            //
            var loadEngine = (ILoadEngine) loadEngineInfo.CreateLoadEngine();

            //
            Assert.IsNotNull(loadEngine);
        }
    }

    internal class LoadEngine0 : ILoadEngine
    {
        public ArchiCopGraph<ArchiCopVertex> LoadGraph()
        {
            throw new NotImplementedException();
        }
    }

    internal class LoadEngine1 : ILoadEngine
    {
        public LoadEngine1(string arg1)
        {
        }

        public ArchiCopGraph<ArchiCopVertex> LoadGraph()
        {
            throw new NotImplementedException();
        }
    }

    internal class LoadEngine2 : ILoadEngine
    {
        public LoadEngine2(string arg1, string arg2)
        {
        }

        public ArchiCopGraph<ArchiCopVertex> LoadGraph()
        {
            throw new NotImplementedException();
        }
    }
}