#region Licence

// Description: SymuBiz - SymuToolsTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace SymuToolsTests.Serialization
{
    public class SerializationTest
    {
        public int Index { get; set; }
        public List<int> List { get; } = new List<int>();

        internal void Add(int index)
        {
            List.Add(index);
            Index++;
        }
    }

    [TestClass]
    public class SerializationTests
    {
        private readonly string filename =
            @"C:\Users\laure\Dropbox\Symu\SymuOrg\Github\Symu source code\Resources\Serialization\test.txt";


        [TestMethod]
        public void ReadAndWriteTest()
        {
            var serializer = new XmlSerializer(typeof(SerializationTest));
            //WRITE
            TextWriter writer = new StreamWriter(filename);
            var teamWrite = new SerializationTest();
            teamWrite.Add(1);
            serializer.Serialize(writer, teamWrite);
            writer.Close();
            writer.Dispose();
            //READ
            /* If the XML document has been altered with unknown 
      nodes or attributes, handle them with the 
      UnknownNode and UnknownAttribute events.*/
            serializer.UnknownNode += Serializer_UnknownNode;
            serializer.UnknownAttribute += Serializer_UnknownAttribute;
            // A FileStream is needed to read the XML document.
            var fs = new FileStream(filename, FileMode.Open);
            // Declare an object variable of the type to be deserialized.
            SerializationTest teamRead;
            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
            teamRead = (SerializationTest) serializer.Deserialize(fs);
            fs.Dispose();

            Assert.AreEqual(1, teamRead.Index);
            Assert.AreEqual(1, teamRead.List.Count);
        }

        private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void Serializer_UnknownAttribute
            (object sender, XmlAttributeEventArgs e)
        {
            var attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
                              attr.Name + "='" + attr.Value + "'");
        }
    }
}