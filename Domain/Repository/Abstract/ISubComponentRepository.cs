using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface --------------------*
 *                                                            *
 * This interface is for the interaction with Database in     *
 * contex with Project's Component's  Sub-Component relevant  *
 * data manipulation. Please see "SqlSubComponentRepository"  *
 * class for further details.                                 * 
 *                                                            *
 *------------------------------------------------------------*/

namespace Domain.Repository.Abstract
{
    public interface ISubComponentRepository
    {
        IQueryable<SubCompAttValue> GetSubComponents(long pCompID);
        IQueryable<SubCompAttValue> GetSubComponentByID(long pSubCompID);
        IQueryable<FieldType> GetFieldTypes();
        IQueryable<RegularExpression> GetRegularExpressions();
        IQueryable<SubCompAttribute> GetSubComponentAttributes(List<int> pCustomAttributes);
        IQueryable<SubCompAttribute> GetSubComponentAttributes(bool pOnlyCustomLevel);
        bool CreateSubComponent(string[] pAttValues, int[] pAttIDs, long pCompID);
        bool EditSubComponent(string[] pAttValues, int[] pAttIDs, long pSubComponentID);
        int CreateCustomField(string[] data);
        SubCompAttribute GetFieldByID(int pFieldID);
        int EditField(int pFieldID, string pFieldName, string pListOptions);
        bool IsFieldExists(string pFieldName, int pFieldID);
        IQueryable<SubCompAttValue> SearchSubComponent(int pFieldID, string pFieldValue, long pCompID);
    }
}
