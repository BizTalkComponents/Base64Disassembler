using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using System.Text;
using System.Xml;

namespace BizTalkComponents.Base64Disassembler.Test.UnitTest
{
    [TestClass]
    public class Base64DisassemblerTest
    {
        [TestMethod]
        public void TestHappyPath()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            string m = "<body></body>";

            var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(m));
            var message = MessageHelper.CreateFromString(string.Format("<envelope><msg>{0}</msg></envelope>", b64));

            var disassembler = new PipelineComponents.Base64Disassembler.Base64Disassembler
            {
                Xpath = "/envelope/msg"
            };

            pipeline.AddComponent(disassembler, PipelineStage.Disassemble);

            var result = pipeline.Execute(message);

            var doc = new XmlDocument();
            doc.Load(result[0].BodyPart.Data);

            Assert.AreEqual(m, doc.OuterXml);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidXpath()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            string m = "<body></body>";

            var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(m));
            var message = MessageHelper.CreateFromString(string.Format("<envelope><msg>{0}</msg></envelope>", b64));

            var disassembler = new PipelineComponents.Base64Disassembler.Base64Disassembler
            {
                Xpath = "/invalid/msg"
            };

            pipeline.AddComponent(disassembler, PipelineStage.Disassemble);

            var result = pipeline.Execute(message);
        }
    }
}
