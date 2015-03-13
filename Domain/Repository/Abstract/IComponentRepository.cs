using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface --------------------*
 *                                                            *
 * This interface is for the interaction with Database in     *
 * contex with Project's Components relevant data manipulati- *
 * on. Please see "SqlComponentRepository" class for further  *
 * details.                                                   * 
 *                                                            *
 *------------------------------------------------------------*/

namespace Domain.Repository.Abstract
{
    public interface IComponentRepository
    {
        IQueryable<CompAttValue> GetProjectComponents(long pProjectID);
        IQueryable<CompAttValue> GetComponentByID(long pCompID);
        IQueryable<FieldType> GetFieldTypes();
        IQueryable<RegularExpression> GetRegularExpressions();
        IQueryable<CompAttribute> GetComponentAttributes(List<int> pCustomAttributes);
        IQueryable<CompAttribute> GetComponentAttributes(bool pOnlyCustomLevel);
        bool CreateComponent(string[] pAttValues, int[] pAttIDs, long pProjectID);
        string GetComponentName(long pCompID);
        bool EditComponent(string[] pAttValues, int[] pAttIDs, long pComponentID);
        int CreateCustomField(string[] data);
        int EditField(int pFieldID, string pFieldName, string pListOptions);
        CompAttribute GetFieldByID(int pFieldID);
        bool IsFieldExists(string pFieldName, int pFieldID);
        IQueryable<CompAttValue> SearchComponent(int pFieldID, string pFieldValue, long pPorjectID);
    }
}
