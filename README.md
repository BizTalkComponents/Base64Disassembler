[![Build status](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/Base64Disassembler?branch=master)](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/Base64Disassembler/branch/master)

## Description ##
The Base64Disassembler component replaces the incomming message with a new message from a base64 encoded string at a specified location in the incomming message.


| Parameter      | Description                                               | Type | Validation|
| ---------------|-----------------------------------------------------------|------|-----------|
|Xpath|Xpath expression pointing to the node to read the base64 string from.|String|Required|

## Remarks ##
If the Xpath is not set an ArgumentException will be thrown. If there are no node matching the xpath an exception will be thrown.
