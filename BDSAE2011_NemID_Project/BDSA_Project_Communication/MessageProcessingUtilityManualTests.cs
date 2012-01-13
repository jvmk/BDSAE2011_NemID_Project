// -----------------------------------------------------------------------
// <copyright file="MessageProcessingUtilityManualTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSAE2011_NemID_Project
{
    using System;
    using System.IO;
    using System.Text;

    using BDSA_Project_Communication;

    using NUnit.Framework;

    /// <summary>
    /// Tests methods in MessageProcessingUtility that do not require generated keys.
    /// <author>Janus Varmarken</author>
    /// </summary>
    [TestFixture]
    public class MessageProcessingUtilityManualTests
    {
        [Test]
        public void TestReadFrom()
        {
            // write text to stream
            MemoryStream outputStream = new MemoryStream();
            string inputText = "Hello, world!";
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);
            outputStream.Write(inputBytes, 0, inputBytes.Length);

            // use ReadFrom to recieve the data
            string outputString = MessageProcessingUtility.ReadFrom(outputStream);
            Console.WriteLine(outputString);
            Assert.True(outputString.Equals(inputText));
            outputStream.Close();
        }

        [Test]
        public void TestDoesUrlContainRequest()
        {
            string validUri = "/somepage&request=somerequest";
            string invalidUri = "/login";
            Assert.True(MessageProcessingUtility.DoesUrlContainRequest(validUri));
            Assert.False(MessageProcessingUtility.DoesUrlContainRequest(invalidUri));
        }

        [Test]
        public void TestIsValidUrl()
        {
            // (these tests fail if scheme is set to http inside the IsValidUrl method in MessageProcessingUtility)
            Assert.True(MessageProcessingUtility.IsValidUrl("https://localhost:8080/")); // accepts ports
            Assert.True(MessageProcessingUtility.IsValidUrl("https://somepage.dk/")); // accepts domains
            Assert.True(MessageProcessingUtility.IsValidUrl("https://localhost:8080/subpage")); // subpage ok
            Assert.False(MessageProcessingUtility.IsValidUrl("http://localhost:8080")); // scheme must be https
            Assert.False(MessageProcessingUtility.IsValidUrl("https://localhost:8080")); // is end-slash needed?
        }

        [Test]
        public void TestGetRequesterOperation()
        {
            string wellFormed1 = "/request=myreq/";
            string wellFormed2 = "/request=redirect&someemoreinfo/";
            Assert.True(MessageProcessingUtility.GetRequesterOperation(wellFormed1).Equals("myreq"));
            Assert.True(MessageProcessingUtility.GetRequesterOperation(wellFormed2).Equals("redirect"));
        }

        [Test]
        public void TestIsRawMessageBodyWellFormed()
        {

            string wellformed = "origin=adkgfjkjgjkjg&fkjsgkfgkjgkjfggjjh&hdhgfhdfhd";
            Assert.True(MessageProcessingUtility.IsRawMessageBodyWellFormed(wellformed)); // has 'origin=' and 2 '&' 

            string endsWithAmbs = "origin=adkgfjkjgjkjg&fkjsgkfgkjgkjfggjjhhdhgfhdfhd&";
            Assert.True(MessageProcessingUtility.IsRawMessageBodyWellFormed(endsWithAmbs)); // empty, but still legal, request

            Assert.False(MessageProcessingUtility.IsRawMessageBodyWellFormed(null)); // null is not allowed (enter 1st if(!wellFormed))

            string noAmbs = "origin=kdgjjjjjjjjjjjjjjjjjjjjklkæjjsfsfdasfafadfa";
            Assert.False(MessageProcessingUtility.IsRawMessageBodyWellFormed(noAmbs)); // '&' not found (enter 2nd if(!wellFormed))

            string notBase64Origin = "origin=lkdagkagkl_4343&adcadfdaf&hsdhdfh";
            Assert.False(MessageProcessingUtility.IsRawMessageBodyWellFormed(notBase64Origin)); // '_' is not a valid Base64 (enter 3rd if(!wellFormed))

            string oneAmbs = "origin=adfsadfaf&agdgsgsfgfhfshfshsfhsfhsf";
            Assert.False(MessageProcessingUtility.IsRawMessageBodyWellFormed(oneAmbs)); // Only one '&' (enter 4th if(!wellFormed))

            string threeAmbs = "origin=adkgfjkjgjkjg&fkjsgkfgkjgk&jfggjjh&hdhgfhdfhd";
            Assert.False(MessageProcessingUtility.IsRawMessageBodyWellFormed(threeAmbs)); // only 2 x '&' allowed
        }
    }
}