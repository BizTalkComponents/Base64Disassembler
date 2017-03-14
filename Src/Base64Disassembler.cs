using Microsoft.BizTalk.Component.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.BizTalk.Message.Interop;
using System.ComponentModel;
using BizTalkComponents.Utils;
using System.Xml;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.XPath;
using System.IO;

namespace BizTalkComponents.PipelineComponents.Base64Disassembler
{
    [System.Runtime.InteropServices.Guid("8E1B9966-523B-4C48-B894-E1E53BCA4770")]
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    public partial class Base64Disassembler : IBaseComponent,
                                        IPersistPropertyBag, IComponentUI, IDisassemblerComponent
    {
        private const string XpathPropertyName = "Xpath";
        private readonly Queue _outputQueue = new Queue();


        [DisplayName("Xpath")]
        [Description("Xpath from wich to extract body from.")]
        [RequiredRuntime]
        public string Xpath { get; set; }

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var data = pInMsg.BodyPart.GetOriginalDataStream();

            const int bufferSize = 0x280;
            const int thresholdSize = 0x100000;

            if (!data.CanSeek || !data.CanRead)
            {
                data = new ReadOnlySeekableStream(data, new VirtualStream(bufferSize, thresholdSize), bufferSize);
                pContext.ResourceTracker.AddResource(data);
            }
            XmlTextReader xmlTextReader = new XmlTextReader(data);
            XPathCollection xPathCollection = new XPathCollection();
            XPathReader xPathReader = new XPathReader(xmlTextReader, xPathCollection);
            xPathCollection.Add(Xpath);
            var ms = new MemoryStream();
            int readBytes = 0;
            bool found = false;
            while (xPathReader.ReadUntilMatch())
            {
                if (xPathReader.Match(0))
                {
                    var bw = new BinaryWriter(ms);
                    byte[] buffer = new byte[bufferSize];

                    while ((readBytes = xPathReader.BaseReader.ReadElementContentAsBase64(buffer, 0, bufferSize)) > 0)
                    {
                        bw.Write(buffer);
                    }
                    bw.Flush();
                    found = true;
                    break;
                }
            }
            
            if(!found)
            {
                throw new ArgumentException("Could not element at specified xpath");
            }

            ms.Seek(0, SeekOrigin.Begin);
            ms.Position = 0;

            var docType = Microsoft.BizTalk.Streaming.Utils.GetDocType(new MarkableForwardOnlyEventingReadStream(ms));

            pInMsg.Context.Promote(new ContextProperty(SystemProperties.MessageType), docType);
            pInMsg.BodyPart.Data = ms;     
            _outputQueue.Enqueue(pInMsg);
        }


        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            if (_outputQueue.Count > 0)
            {
                return (IBaseMessage)_outputQueue.Dequeue();
            }

            return null;
        }


        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            Xpath = PropertyBagHelper.ReadPropertyBag(propertyBag, XpathPropertyName, Xpath);
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, XpathPropertyName, Xpath);
        }

    }
}
