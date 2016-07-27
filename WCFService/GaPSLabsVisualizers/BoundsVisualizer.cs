/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using Aram.OSMParser;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: DebuggerVisualizer(typeof(GaPSlabsVisualizersBounds.BoundsVisualizer), typeof(VisualizerObjectSource), Target = typeof(Bounds))]
namespace GaPSlabsVisualizersBounds
{
    // TODO: Add the following to SomeType's definition to see this visualizer when debugging instances of SomeType:
    // 
    //  [DebuggerVisualizer(typeof(ServiceBoundsVisualizer))]
    //  [Serializable]
    //  public class SomeType
    //  {
    //   ...
    //  }
    // 
    /// <summary>
    /// A Visualizer for SomeType.  
    /// </summary>
    public class BoundsVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            // TODO: Get the object to display a visualizer for.
            //       Cast the result of objectProvider.GetObject() 
            //       to the type of the object being visualized.
            var data = (Bounds)objectProvider.GetObject();

            // TODO: Display your view of the object.
            //       Replace displayForm with your own custom Form or Control.
            using (GaPSlabsVisualizers.BoundsForm displayForm = new GaPSlabsVisualizers.BoundsForm())
            {
                displayForm.textBoxLatMin.Text = data.minlat + "";
                displayForm.textBoxLatMax.Text = data.maxlat + "";
                displayForm.textBoxLonMin.Text = data.minlon + "";
                displayForm.textBoxLonMax.Text = data.maxlon + "";
                windowService.ShowDialog(displayForm);
            }
        }

        // TODO: Add the following to your testing code to test the visualizer:
        // 
        //    ServiceBoundsVisualizer.TestShowVisualizer(new SomeType());
        // 
        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(BoundsVisualizer));
            visualizerHost.ShowVisualizer();
        }
    }
}
