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
 * in contex with Project's Component's Sub-Components rele- *
 * vant data manipulation. Like: Creating and Updating the   *
 * sub-componet. Creating and editing custom fields and lik- *
 * ewise some other operations.                              *
 *                                                           *
 *-----------------------------------------------------------*/

namespace Domain.Repository.Concrete
{
    public class SqlSubComponentRepository : ISubComponentRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlSubComponentRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public IQueryable<SubCompAttValue> GetSubComponents(long pCompID)
        {
            var subCompIDs = (from x in _objectModel.SubComponents
                           where x.CompID == pCompID
                          select x.SubCompID).ToList();

            if (subCompIDs.Count() > 0)
            {
                var subComponentsNames = from x in _objectModel.SubCompAttributes
                                      where x.SubCompAttName == "Sub-Component Name"
                                      from y in _objectModel.SubCompAttValues
                                      where subCompIDs.Contains(y.SubCompID) && y.SubCompAttID == x.SubCompAttID
                                      select y;

                return subComponentsNames;
            }

            return null;
        }

        public IQueryable<SubCompAttValue> GetSubComponentByID(long pSubCompID)
        {
            return _objectModel.SubCompAttValues.Where(x => x.SubCompID == pSubCompID);
        }

        public IQueryable<FieldType> GetFieldTypes()
        {
            return _objectModel.FieldTypes;
        }

        public IQueryable<RegularExpression> GetRegularExpressions()
        {
            return _objectModel.RegularExpressions;
        }

        public IQueryable<SubCompAttribute> GetSubComponentAttributes(List<int> pCustomAttributes)
        {
            if (pCustomAttributes.Count > 0)
            {
                var attributes = (from x in _objectModel.SubCompAttributes
                                  where (x.IsSystemLevel == true) || pCustomAttributes.Contains(x.SubCompAttID)
                                  select x).OrderBy(x => x.FieldType);

                return attributes;
            }

            else
            {
                return _objectModel.SubCompAttributes.Where(x => x.IsSystemLevel == true).OrderBy(x => x.FieldType);
            }
        }

        public IQueryable<SubCompAttribute> GetSubComponentAttributes(bool pOnlyCustomLevel)
        {
            if (pOnlyCustomLevel)
            {
                return _objectModel.SubCompAttributes.Where(x => x.IsSystemLevel == false);
            }

            else
            {
                return _objectModel.SubCompAttributes;
            }
        }

        public bool CreateSubComponent(string[] pAttValues, int[] pAttIDs, long pCompID)
        {
            int index = 0;
            long subCompID;
            bool result = false;

            _objectModel.SubComponents.InsertOnSubmit(new SubComponent
                {
                    CompID = pCompID
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
                // Getting the newly created sub component id.
                subCompID = _objectModel.SubComponents.Select(x => x.SubCompID).Max();

                foreach (int id in pAttIDs)
                {
                    _objectModel.SubCompAttValues.InsertOnSubmit(new SubCompAttValue
                        {
                            SubCompAttID = id,
                            SubCompID = subCompID,
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

        public bool EditSubComponent(string[] pAttValues, int[] pAttIDs, long pSubCompID)
        {
            int index = 0;
            bool result = false;

            var subComponent = from x in _objectModel.SubCompAttValues
                                where x.SubCompID == pSubCompID
                                select x;

            foreach (int id in pAttIDs)
            {
                subComponent.Where(x => x.SubCompAttID == id).First().Value = pAttValues[index++];
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

            var attribute = from x in _objectModel.SubCompAttributes
                            where x.SubCompAttName == data[0]
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
                    _objectModel.SubCompAttributes.InsertOnSubmit(new SubCompAttribute
                    {
                        SubCompAttName = data[0],
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
                    _objectModel.SubCompAttributes.InsertOnSubmit(new SubCompAttribute
                        {
                            SubCompAttName = data[0],
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

        public SubCompAttribute GetFieldByID(int pFieldID)
        {
            return _objectModel.SubCompAttributes.Where(x => x.SubCompAttID == pFieldID).First();
        }

        // User can only change the Field Name property. And, if the field is of
        // type list then user can specify new option(s) for list which will be added
        // in existing list. So, third parameter is for that purpose.
        public int EditField(int pFieldID, string pFieldName, string pListOptions)
        {
            int result = -1;

            var attribute = from x in _objectModel.SubCompAttributes
                            where x.SubCompAttName == pFieldName && x.SubCompAttID != pFieldID
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                var subCompAtt = (from x in _objectModel.SubCompAttributes
                                 where x.SubCompAttID == pFieldID
                                 select x).First();

                subCompAtt.SubCompAttName = pFieldName;

                if (pListOptions != null && pListOptions != "")
                {
                    subCompAtt.DefaultValue = subCompAtt.DefaultValue + ";" + pListOptions;
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
            IQueryable<SubCompAttribute> field;

            if (pFieldID == 0)
            {
                field = from x in _objectModel.SubCompAttributes
                        where x.SubCompAttName == pFieldName
                        select x;
            }

            else
            {
                field = from x in _objectModel.SubCompAttributes
                        where x.SubCompAttName == pFieldName && x.SubCompAttID != pFieldID
                        select x;
            }

            return (field.Count() != 0);
        }

        public IQueryable<SubCompAttValue> SearchSubComponent(int pFieldID, string pFieldValue, long pCompID)
        {
            int fieldType = 0;
            IQueryable<long> subComponentsIDs;

            // The value 0 is for search by Sub-Component ID.
            // For this, we have to query with SubComponents' table
            // rather than with SubCompAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.SubCompAttributes
                             where f.SubCompAttID== pFieldID
                             select f.FieldType).First();
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                subComponentsIDs = from x in _objectModel.SubCompAttValues
                                   where x.SubComponent.CompID == pCompID && x.SubCompAttID == pFieldID && x.Value == null
                                   select x.SubCompID;
            }

            else
            {
                // If pFieldID = 0 then this means the search is on Sub-Component ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);

                    subComponentsIDs = from x in _objectModel.SubComponents
                                       where x.CompID == pCompID && x.SubCompID == id
                                       select x.SubCompID;
                }

                else
                {
                    subComponentsIDs = from x in _objectModel.SubCompAttValues
                                       where x.SubComponent.CompID == pCompID && x.SubCompAttID == pFieldID && x.Value == pFieldValue
                                       select x.SubCompID;
                }
            }

            if (subComponentsIDs.Count() > 0)
            {
                var subComponentsNames = from x in _objectModel.SubCompAttributes
                                         where x.SubCompAttName == "Sub-Component Name"
                                         from y in _objectModel.SubCompAttValues
                                         where y.SubCompAttID == x.SubCompAttID && subComponentsIDs.Contains(y.SubCompID)
                                         select y;

                return subComponentsNames;
            }

            return null;
        }
    }
}
