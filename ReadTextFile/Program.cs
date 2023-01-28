using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReadTextFile
{
    class Program
    {
        private static int cout = 0;

        static void Main()
        {
            List<DateStation> dateStationList = new List<DateStation>();
            List<SynchronousMachine> synchronousMachine = new List<SynchronousMachine>();
            List<SynchronousMachine> synMachines = new List<SynchronousMachine>();
            string str = "";

            XElement root = XElement.Load("C:\\Users\\User\\Desktop\\Example.xml");
            XNamespace cim = "http://iec.ch/TC57/2014/CIM-schema-cim16#";
            XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

            IEnumerable<XElement> idSubstation =
               from element in root.Elements(cim + "Substation")
               select element;
            foreach (XElement element in idSubstation) 
            {
                dateStationList.Add(new DateStation(element.Attribute(rdf + "about").Value.ToString(), "", "", "", ""));
                synMachines.Add(new SynchronousMachine(element.Attribute(rdf + "about").Value.ToString(), ""));
            }
                


            IEnumerable<XElement> substation =
               from element in root.Elements(cim + "Substation")
               select element.Element(cim + "IdentifiedObject.name");

            foreach (XElement element in substation)
            {
                dateStationList[cout].SetSubstatioName(element.Value);
                cout++;

            }
            cout = 0;

            for (int i = 0; i < dateStationList.Count; i++)
            {

                IEnumerable<XElement> idVoltageLevels =
                                from element in root.Descendants(cim + "Substation")
                                where (string)element.Attribute(rdf + "about") == dateStationList[i].GetSubstationID()
                                select element.Element(cim + "Substation.VoltageLevels");
                foreach (XElement element in idVoltageLevels)
                {
                    if (element != null)
                    {
                        dateStationList[i].SetGeneratorID(element.Attribute(rdf + "resource").Value);
                    }
                    else
                    {
                        dateStationList[i].SetGeneratorID("Элемент не найден!");
                    }
                }

                //запрос на получение напряжения генератора
                IEnumerable<XElement> voltageLevel =
                from element in root.Descendants(cim + "VoltageLevel")
                where (string)element.Attribute(rdf + "about") == dateStationList[i].GetGeneratorID()
                select element.Element(cim + "IdentifiedObject.name");

                if (!voltageLevel.Any())
                {
                    dateStationList[i].SetVoltageLevel("Нет значения!");
                }

                foreach (XElement element in voltageLevel)
                {
                    if (!element.Value.Equals(""))
                    {
                        dateStationList[i].SetVoltageLevel(element.Value);
                    }
                }
            }

            IEnumerable<XElement> synchronousMachineID =
                    from element in root.Descendants(cim + "SynchronousMachine")
                    select element;
            foreach (XElement el in synchronousMachineID)
            {
                if (el != null)
                    synchronousMachine.Add(new SynchronousMachine(el.Attribute(rdf + "about").Value, "", ""));
            }

            for (int i = 0; i < synchronousMachine.Count; i++)
            {
                //получение в SynchronousMachine у поля Equipment.EquipmentContainer его id
                IEnumerable<XElement> equipmentContainer =
                       from element in root.Descendants(cim + "SynchronousMachine")
                       where (string)element.Attribute(rdf + "about") == synchronousMachine[i].GetSynchronousMachineID()
                       select element.Element(cim + "Equipment.EquipmentContainer");
                foreach (XElement el in equipmentContainer)
                {
                    if (el != null)
                        synchronousMachine[i].SetEquipmentContainer(el.Attribute(rdf + "resource").Value);
                }

                //получение в SynchronousMachine типа генератора

                IEnumerable<XElement> synchronousMachineType =
                       from element in root.Descendants(cim + "SynchronousMachine")
                       where (string)element.Attribute(rdf + "about") == synchronousMachine[i].GetSynchronousMachineID()
                       select element.Element(cim + "IdentifiedObject.name");
                foreach (XElement el in synchronousMachineType)
                {
                    synchronousMachine[i].SetTypeGeneratorName(el.Value);
                }
            }

            for (int i = 0; i < synchronousMachine.Count; i++)
            {
                for (int j = 0; j < dateStationList.Count; j++)
                {
                    if (synchronousMachine[i].GetEquipmentContainer().Equals(dateStationList[j].GetGeneratorID()))
                    {
                        dateStationList[j].SetGeneratorName(synchronousMachine[j].GetTypeGeneratorName());
                    }
                }
            }




            for (int i = 0;i<synMachines.Count();i++) { 
                IEnumerable<XElement> idVoltageLevel =
                                 from element in root.Elements(cim + "Substation")
                                 where (string)element.Attribute(rdf + "about") == synMachines[i].GetSynSubstationID()
                                 select element;
                if (!idVoltageLevel.Any())
                {
                    dateStationList[i].SetGeneratorID("Элемент не найден!");
                }

                foreach (XElement element in idVoltageLevel)
                {
                    IEnumerable<XElement> elem = element.Elements(cim + "Substation.VoltageLevels");
                    foreach (XElement xElem in elem)
                    {
                        if (xElem != null)
                        {
                            if (elem.Count() > 1)
                            {
                                str += xElem.Attribute(rdf + "resource").Value + " ";
                            }
                            else
                            {
                                str = xElem.Attribute(rdf + "resource").Value;
                            }
                        }
                    }
                }
                synMachines[i].SetVoltageLevelsIDCount(str);
                str = "";
            }

            for (int i = 0;i<synMachines.Count();i++) 
            {
                string[] arr = synMachines[i].GetVoltageLevelsIDCount().Split(new char[] { ' ' });
                foreach (string stringsID in arr)
                {
                    for (int j = 0; j < synchronousMachine.Count(); j++) 
                    {
                        if (synchronousMachine[j].GetEquipmentContainer().Equals(stringsID)) 
                        {
                            str += synchronousMachine[j].GetTypeGeneratorName() + " "; 
                        }  
                    }
                   
                }

                if (str.Equals(""))
                {
                    dateStationList[i].SetGeneratorName("Тип не найден!");
                }
                else 
                {
                   dateStationList[i].SetGeneratorName(str); 
                }
                str = string.Empty;
            }


         /*   foreach (SynchronousMachine machine in synchronousMachine)
            {
                Console.WriteLine(machine.GetEquipmentContainer() + "   " + machine.GetTypeGeneratorName() + " " + machine.GetSynchronousMachineID());
            }
            Console.WriteLine("___________________________________________________");*/
            foreach (DateStation station in dateStationList)
            {
                Console.WriteLine(station.GetSubstationID() + " | " + station.GetGeneratorID() + "|" + station.GetVoltageLevel() + " | " + station.GetNameSubstation() + "|" + station.GetGeneratorName());
            }

         /*   foreach (DateStation dateSt in dateStationList)
            {
                Console.WriteLine(dateSt.GetGeneratorName());
            }*/


            Console.ReadLine();
            }
        }
    }
    internal class  DateStation 
    {
        private string substationID;        //id станции
        private string nameSubstation;      //название станции
        private string voltageLevel;        //название распределительного устройства
        private string nameGenerator;       //название генератора 
        private string generatorID;         // id генератора
        
        public DateStation(string substationID, string nameSubstation,string generatorID, string voltageLevel, string nameGenerator)
        {
            this.SetSubstatioName(nameSubstation);
            this.SetVoltageLevel(voltageLevel);
            this.SetGeneratorName(nameGenerator);
            this.SetGeneratorID(generatorID);
            this.SetSubstationID(substationID);
        }

        public string GetSubstationID()
        {
            return substationID;
        }

        public void SetSubstationID(string value)
        {
            substationID = value;
        }

        public string GetGeneratorID()
        {
            return generatorID;
        }

        public void SetGeneratorID(string value)
        {
            generatorID = value;
        }

        public string GetNameSubstation()
        {
            return nameSubstation;
        }

        public void SetSubstatioName(string value)
        {
            nameSubstation = value;
        }

        public string GetVoltageLevel()
        {
            return voltageLevel;
        }

        public void SetVoltageLevel(string value)
        {
            voltageLevel = value;
        }

        public string GetGeneratorName()
        {
            return nameGenerator;
        }

        public void SetGeneratorName(string value)
        {
            nameGenerator = value;
        }
    }

    internal class SynchronousMachine
    {
        private string equipmentContainer;      //id на генератор
        private string typeGeneratorName;       //тип генератора
        private string synchronousMachineID;    //id блока данных, где id и тип генератор
        private string voltageLevelsIDCount;    //несколько id в одном поле
        private string synSubstationID;            //id станций
    public SynchronousMachine(string substationID, string voltageLevelsIDCount)
    {
        this.synSubstationID = substationID;
        this.voltageLevelsIDCount = voltageLevelsIDCount; 
    }

    public SynchronousMachine(string synchronousMachineID, string equipmentContainer, string typeName)
    {
        this.equipmentContainer = equipmentContainer;
        this.typeGeneratorName = typeName;
        this.synchronousMachineID = synchronousMachineID;
    }

    public string GetSynSubstationID()
    {
        return synSubstationID;
    }

    public void SetSynSubstationID(string value)
    {
        synSubstationID = value;
    }

    public string GetVoltageLevelsIDCount()
    {
        return voltageLevelsIDCount;
    }

    public void SetVoltageLevelsIDCount(string value)
    {
        voltageLevelsIDCount = value;
    }

    public string GetSynchronousMachineID()
    {
        return synchronousMachineID;
    }

    public void SetSynchronousMachineID(string value)
    {
        synchronousMachineID = value;
    }

    public string GetEquipmentContainer()
        {
            return equipmentContainer;
        }

        public void SetEquipmentContainer(string value)
        {
            equipmentContainer = value;
        }

        public string GetTypeGeneratorName()
        {
            return typeGeneratorName;
        }

        public void SetTypeGeneratorName(string value)
        {
            typeGeneratorName = value;
        }
    }