using System;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Threading;

namespace FamilyEditor
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]


    public class reSave : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Логика плагина

            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;

            string originalPath = doc.PathName;

            Result result;

            //проверка на то, что текущий файл - это файл семейства
            if (doc.IsFamilyDocument)
            {
                //определяю длину адреса файла и добавляю "_" перед ".rfa"
                int originalPathLength = originalPath.Length;
                string time = DateTime.Now.ToString("yy.MM.dd_HH.mm.ss");
                
                string templPath = originalPath.Insert(originalPathLength - 4, "_TempFileTimeIs."+ time);


                //сохраняю с новым именем
                doc.SaveAs(templPath);

                // Приостановка на 2 секунды (2000 миллисекунд)
                Thread.Sleep(2000); 


                //удаляю исходный файл.
                File.Delete(originalPath);

                //потом сохраняю под старым именем
                doc.SaveAs(originalPath);

                //удаляю новый файл
                File.Delete(templPath);                                

                result = Result.Succeeded;
            }
            else
            {
                TaskDialog.Show("Ошибка", "Пересохранить можно только семейство или шаблон проекта\nКоманда не выполнена" +
                    "\nPlease run this command in a family document");
                result = Result.Failed;
            }

            return result;

            #endregion
                 
        }
    }
}

