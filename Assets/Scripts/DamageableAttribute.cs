using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Damageable
{
    public class DamagableAttribute
    {
        public string Key;
        public string Value;
     
        public DamagableAttribute (string key, string val)
        {
            Key = key;
            Value = val;
        }
        
        public DamagableAttribute ()
        {
        }
     
        static private List<DamagableAttribute> convert<T> (Dictionary<string,T> d)
        {
            List<DamagableAttribute> tempDamageAttributes = new List<DamagableAttribute> (d.Count);
 
            foreach (string key in d.Keys)
            {
                tempDamageAttributes.Add (new DamagableAttribute (key, d[key].ToString ()));
            }
         
            return tempDamageAttributes;
        }
        
        static public string serialize<T> (Dictionary<string,T> d)
        {
            List<DamagableAttribute> serializable = convert (d);
            XmlSerializer serializer = new XmlSerializer (typeof(List<DamagableAttribute>));
            StringWriter sw = new StringWriter ();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces ();
            ns.Add ("", "");
         
            serializer.Serialize (sw, serializable, ns);
         
            return sw.ToString ();
        }
        
        static public Dictionary<string,T> deSerialize<T> (string input)
        {
            Dictionary<string,T> output = new Dictionary<string, T> ();
            XmlSerializer xs = new XmlSerializer (typeof(List<DamagableAttribute>));
            StringReader sr = new StringReader (input);
            
            List<DamagableAttribute> intermediateList = (List<DamagableAttribute>) xs.Deserialize (sr);
            
            foreach (DamagableAttribute item in intermediateList)
            {
                output.Add (item.Key, (T) Convert.ChangeType (item.Value, typeof(T)));
            }
            
            return output;
        }
    }
}
