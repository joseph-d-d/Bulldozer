﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bulldozer
{
    /// <summary>
    /// Interaction logic for SelectPage.xaml
    /// </summary>
    public partial class SelectPage : Page
    {
        private BulldozerComponent bulldozer;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectPage"/> class.
        /// </summary>
        public SelectPage( BulldozerComponent parameter = null )
        {
            InitializeComponent();
            if ( parameter != null )
            {
                bulldozer = parameter;
                if ( bulldozer.DataNodes.Count > 0 )
                {
                    bulldozer.DataNodes[0].Checked = true; //preview on load
                    PreviewData( bulldozer.DataNodes[0].Id );
                }
                treeView.ItemsSource = new ObservableCollection<DataNode>( bulldozer.DataNodes );
            }
            else
            {
                lblNoData.Visibility = Visibility.Visible;
                btnNext.Visibility = Visibility.Hidden;
                grdPreviewData.Visibility = Visibility.Hidden;
            }
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Called when the checkbox is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Checkbox_OnClick( object sender, MouseButtonEventArgs e )
        {
            var selected = (CheckBox)sender;
            if ( selected != null )
            {
                SelectedId.Id = selected.Uid;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the TextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TextBlock_MouseDown( object sender, MouseButtonEventArgs e )
        {
            var textBlock = (TextBlock)sender;
            if ( textBlock != null )
            {
                PreviewData( (string)textBlock.Tag );
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the TextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TextBlock_KeyDown( object sender, KeyEventArgs e )
        {
            var textBlock = (TextBlock)sender;
            if ( textBlock != null )
            {
                PreviewData( (string)textBlock.Tag );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSelectAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSelectAll_Click( object sender, RoutedEventArgs e )
        {
            foreach ( var node in bulldozer.DataNodes )
            {
                node.Checked = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnUnselectAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnUnselectAll_Click( object sender, RoutedEventArgs e )
        {
            foreach ( var node in bulldozer.DataNodes )
            {
                node.Checked = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnBack_Click( object sender, RoutedEventArgs e )
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Handles the Click event of the btnNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnNext_Click( object sender, RoutedEventArgs e )
        {
            var transformPage = new ConfigurationPage( bulldozer );
            this.NavigationService.Navigate( transformPage );
        }

        #endregion Events

        #region Async Tasks

        /// <summary>
        /// Previews the data for the selected node.
        /// </summary>
        /// <param name="tableNode">The table node.</param>
        private void PreviewData( string selectedNodeId )
        {
            BackgroundWorker bwLoadPreview = new BackgroundWorker();
            bwLoadPreview.DoWork += bwLoadPreview_DoWork;
            bwLoadPreview.RunWorkerCompleted += bwLoadPreview_RunWorkerCompleted;
            bwLoadPreview.RunWorkerAsync( selectedNodeId );
        }

        /// <summary>
        /// Handles the DoWork event of the bwLoadPreview control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void bwLoadPreview_DoWork( object sender, DoWorkEventArgs e )
        {
            e.Result = bulldozer.PreviewData( (string)e.Argument );
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the bwLoadPreview control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void bwLoadPreview_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( e.Error == null && (DataTable)e.Result != null )
            {
                DataTable tablePreview = e.Result as DataTable;
                this.Dispatcher.Invoke( (Action)( () =>
                {
                    grdPreviewData.ItemsSource = tablePreview.DefaultView;
                    grdPreviewData.Visibility = Visibility.Visible;
                    lblEmptyDataset.Visibility = Visibility.Hidden;
                } ) );
            }
            else
            {
                lblEmptyDataset.Visibility = Visibility.Visible;
                grdPreviewData.Visibility = Visibility.Hidden;
            }
        }

        #endregion Async Tasks
    }
}
