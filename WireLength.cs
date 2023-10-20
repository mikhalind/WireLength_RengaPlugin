using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renga;

namespace WireLength
{
    public class WireLength : Renga.IPlugin
    {
        private Renga.Application app;

        private Renga.ActionEventSource LengthEventSource; 
        private Renga.IAction LengthAction;

        private string folderPath;

        public bool Initialize(string pluginFolder)
        {
            app = new Renga.Application();
            folderPath = pluginFolder;
            Renga.IUI ui = app.UI;

            try
            {
                InitButtons(ui);
            }
            catch (Exception ex)
            {
                ui.ShowMessageBox(Renga.MessageIcon.MessageIcon_Error, "Error", ex.Message);
                return false;
            }
            return true;
        }

        private void InitButtons(Renga.IUI ui)
        {
            Renga.IImage wIcon = ui.CreateImage();
            wIcon.LoadFromFile(Path.Combine(folderPath, @"Icons\wiring.png"));

            LengthAction = ui.CreateAction();
            LengthAction.DisplayName = "Длина выделенного провода";
            LengthAction.Checkable = false;
            LengthAction.Icon = wIcon;

            LengthEventSource = new Renga.ActionEventSource(LengthAction);
            LengthEventSource.Triggered += LengthAction_Triggered;

            Renga.IUIPanelExtension panel = ui.CreateUIPanelExtension();
            panel.AddToolButton(LengthAction);
            ui.AddExtensionToPrimaryPanel(panel);
        }

        private void LengthAction_Triggered(object sender, EventArgs e)
        {
            Renga.ISelection selection = app.Selection;
            Array selectedObjects = selection.GetSelectedObjects();
            Renga.IModelObjectCollection objCollection = app.Project.Model.GetObjects();
            
            double length = 0;
            for (int i = 0; i < selectedObjects.GetLength(0); i++)
            {
                Renga.IModelObject obj = objCollection.GetById((int)selectedObjects.GetValue(i));
                if (obj.ObjectType == Renga.ObjectTypes.Route)
                {
                    Renga.IQuantity quantVolume = obj.GetQuantities().Get(Renga.QuantityIds.NominalLength);
                    length += quantVolume.AsLength(Renga.LengthUnit.LengthUnit_Meters);
                }
            }
            app.UI.ShowMessageBox(Renga.MessageIcon.MessageIcon_Info, "Информация", $"Выделено {length.ToString("F2")} метров провода");
        }

        public void Stop()
        {
            LengthEventSource.Dispose();
        }
    }
}
