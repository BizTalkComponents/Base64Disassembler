using BizTalkComponents.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.PipelineComponents.Base64Disassembler
{
    public partial class Base64Disassembler
    {
        public string Name { get { return "Base64Disasembler"; } }
        public string Version { get { return "1.0"; } }
        public string Description
        {
            get
            {
                return
                    "Creates a new message from a base64 string at the specified location.";
            }
        }


        public void GetClassID(out Guid classID)
        {
            classID = new Guid("21162095-450F-4A15-A4CF-84C041BDE085");
        }

        public void InitNew()
        {
        }

        public IEnumerator Validate(object projectSystem)
        {
            return ValidationHelper.Validate(this, false).ToArray().GetEnumerator();
        }

        public bool Validate(out string errorMessage)
        {
            var errors = ValidationHelper.Validate(this, true).ToArray();

            if (errors.Any())
            {
                errorMessage = string.Join(",", errors);

                return false;
            }

            errorMessage = string.Empty;

            return true;
        }

        public IntPtr Icon { get { return IntPtr.Zero; } }
    }
}
