using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Repository.Abstract;
using Domain.Entities;
using System.Configuration;

/*-------------------- About this Class ---------------------*
 *                                                           *
 * This class is for the interaction with Database           *
 * in contex with Project's Components relevant data manipul-*
 * ation. Like: Creating and Updating the componet. Creating *
 * and editing custom fields and likewise some other operati-*
 * ons.                                                      *
 *                                                           *
 *-----------------------------------------------------------*/

namespace Domain.Repository.Concrete
{
    public class SqlComponentRepository : IComponentRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlComponentRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public IQueryable<CompAttValue> GetProjectComponents(long pProjectID)
        {
            var compIDs = (from x in _objectModel.Components
                          where x.ProjID == pProjectID
                          select x.CompID).ToList();

            if (compIDs.Count() > 0)
            {
                var componentsNames = from x in _objectModel.CompAttributes
                                      where x.CompAttName == "Component Name"
                                      from y in _objectModel.CompAttValues
                                      where compIDs.Contains(y.CompID) && y.CompAttID == x.CompAttID
                                      select y;

                return componentsNames;
            }

            return null;
        }

        public IQueryable<CompAttValue> GetComponentByID(long pCompID)
        {
            return _objectModel.CompAttValues.Where(x => x.CompID == pCompID);
        }

        public IQueryable<FieldType> GetFieldTypes()
        {
            return _objectModel.FieldTypes;
        }

        public IQueryable<RegularExpression> GetRegularExpressions()
        {
            return _objectModel.RegularExpressions;
        }

        public IQueryable<CompAttribute> GetComponentAttributes(List<int> pCustomAttributes)
        {
            if (pCustomAttributes.Count > 0)
            {
                var attributes = (from x in _objectModel.CompAttributes
                                  where (x.IsSystemLevel == true) || pCustomAttributes.Contains(x.CompAttID)
                                  select x).OrderBy(x => x.FieldType);

                return attributes;
            }

            else
            {
                return _objectModel.CompAttributes.Where(x => x.IsSystemLevel == true).OrderBy(x => x.FieldType);
            }
        }

        public IQueryable<CompAttribute> GetComponentAttributes(bool pOnlyCustomLevel)
        {
            if (pOnlyCustomLevel)
            {
                return _objectModel.CompAttributes.Where(x => x.IsSystemLevel == false);
            }

            else
            {
                return _objectModel.CompAttributes;
            }
        }

        public string GetComponentName(long pCompID)
        {
            var componentName = (from x in _objectModel.CompAttributes
                                  where x.CompAttName == "Component Name"
                                  from y in _objectModel.CompAttValues
                                  where y.CompAttID == x.CompAttID && y.CompID == pCompID
                                  select y.Value).First();

            return componentName;
        }

        public bool CreateComponent(string[] pAttValues, int[] pAttIDs, long pProjectID)
        {
            int index = 0;
            long compID;
            bool result = false;

            _objectModel.Components.InsertOnSubmit(new Component
                {
                    ProjID = pProjectID
                }
            );

            try
            {
                _objectModel.SubmitChanges();
                result = true;
            }

            catch (Exception ex)
            {
            }

            if (result)
            {
                result = false;
                // Getting the newly created component id.
                compID = _objectModel.Components.Select(x => x.CompID).Max();

                foreach (int id in pAttIDs)
                {
                    _objectModel.CompAttValues.InsertOnSubmit(new CompAttValue
                        {
                            CompAttID = id,
                            CompID = compID,
                            Value = pAttValues[index++]
                        }
                    );
                }

                try
                {
                    _objectModel.SubmitChanges();
                    result = true;
                }

                catch (Exception ex)
                {
                }
            }

            return result;
        }

        public bool EditComponent(string[] pAttValues, int[] pAttIDs, long pComponentID)
        {
            int index = 0;
            bool result = false;

            var component = from x in _objectModel.CompAttValues
                            where x.CompID == pComponentID
                            select x;

            foreach (int id in pAttIDs)
            {
                component.Where(x => x.CompAttID == id).First().Value = pAttValues[index++];
            }   

            try
            {
                _objectModel.SubmitChanges();
                result = true;
            }

            catch (Exception ex)
            {
            }

            return result;
        }

        public int CreateCustomField(string[] data)
        {
            int result = -1;

            var attribute = from x in _objectModel.CompAttributes
                            where x.CompAttName == data[0]
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                // If regular expression is not selected then insert null in DB.
                // As the RegularExpression is of type int?, so, it can save null.
                int? nullValue = null;
                int fieldTypeID = Convert.ToInt32(data[1]);

                // If field type is check box or list then some values are null and 
                // need to pass manually.
                if (fieldTypeID != 4 && fieldTypeID != 5)
                {
                    _objectModel.CompAttributes.InsertOnSubmit(new CompAttribute
                    {
                        CompAttName = data[0],
                        FieldType = fieldTypeID,
                        DefaultValue = (data[2] == "") ? null : data[2],
                        CanNull = Convert.ToBoolean(data[3]),
                        RegularExpression = (data[4] == "") ? nullValue : Convert.ToInt32(data[4]),
                        IsSystemLevel = Convert.ToBoolean(data[5])
                    }
                    );
                }

                // For check box and list.
                else
                {
                    _objectModel.CompAttributes.InsertOnSubmit(new CompAttribute
                        {
                            CompAttName = data[0],
                            FieldType = fieldTypeID,
                            // If check box then put null in default value.
                            DefaultValue = (fieldTypeID == 5) ? null : data[2],
                            CanNull = false,
                            RegularExpression = nullValue,
                            IsSystemLevel = Convert.ToBoolean(data[5])
                        }
                    );
                }

                try
                {
                    _objectModel.SubmitChanges();
                    result = 1;
                }

                catch (Exception ex)
                {
                }
            }

            else
            {
                result = 0;
            }

            return result;
        }

        public CompAttribute GetFieldByID(int pFieldID)
        {
            return _objectModel.CompAttributes.Where(x => x.CompAttID == pFieldID).First();
        }

        // User can only change the Field Name property. And, if the field is of
        // type list then user can specify new option(s) for list which will be added
        // in existing list. So, third parameter is for that purpose.
        public int EditField(int pFieldID, string pFieldName, string pListOptions)
        {
            int result = -1;

            var attribute = from x in _objectModel.CompAttributes
                            where x.CompAttName == pFieldName && x.CompAttID != pFieldID
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                var compAtt = (from x in _objectModel.CompAttributes
                               where x.CompAttID == pFieldID
                               select x).First();

                compAtt.CompAttName = pFieldName;

                if (pListOptions != null && pListOptions != "")
                {
                    compAtt.DefaultValue = compAtt.DefaultValue + ";" + pListOptions;
                }

                try
                {
                    _objectModel.SubmitChanges();
                    result = 1;
                }

                catch (Exception ex)
                {
                }
            }

            else
            {
                result = 0;
            }

            return result;
        }

        // The "pFieldID" parameter may contains the field id.
        // This is required in "EditFieldInfo" when user is editing
        // the same field.
        public bool IsFieldExists(string pFieldName, int pFieldID)
        {
            IQueryable<CompAttribute> field;

            if (pFieldID == 0)
            {
                field = from x in _objectModel.CompAttributes
                        where x.CompAttName == pFieldName
                        select x;
            }

            else
            {
                field = from x in _objectModel.CompAttributes
                        where x.CompAttName == pFieldName && x.CompAttID != pFieldID
                        select x;
            }

            return (field.Count() != 0);
        }

        public IQueryable<CompAttValue> SearchComponent(int pFieldID, string pFieldValue, long pProjectID)
        {
            int fieldType = 0;
            IQueryable<long> componentsIDs;

            // The value 0 is for search by Component ID.
            // For this, we have to query with Components' table
            // rather than with CompAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.CompAttributes
                             where f.CompAttID == pFieldID
                             select f.FieldType).First();
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                componentsIDs = from x in _objectModel.CompAttValues
                                where x.Component.ProjID == pProjectID && x.CompAttID == pFieldID && x.Value == null
                                select x.CompID;
            }

            else
            {
                // If pFieldID = 0 then this means the search is on Component ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);

                    componentsIDs = from x in _objectModel.Components
                                    where x.ProjID == pProjectID && x.CompID == id
                                    select x.CompID;
                }

                else
                {
                    componentsIDs = from x in _objectModel.CompAttValues
                                    where x.Component.ProjID == pProjectID && x.CompAttID == pFieldID && x.Value == pFieldValue
                                    select x.CompID;
                }
            }

            if (componentsIDs.Count() > 0)
            {
                var componentsNames = from x in _objectModel.CompAttributes
                                      where x.CompAttName == "Component Name"
                                      from y in _objectModel.CompAttValues
                                      where y.CompAttID == x.CompAttID && componentsIDs.Contains(y.CompID)
                                      select y;

                return componentsNames;
            }

            return null;
        }
    }
}
