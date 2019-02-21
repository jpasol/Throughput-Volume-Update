<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OnOffWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.OnOffInterface = New System.Windows.Forms.DataGridView()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.registry = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.windowstate = New System.Windows.Forms.DataGridViewComboBoxColumn()
        CType(Me.OnOffInterface, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OnOffInterface
        '
        Me.OnOffInterface.AllowUserToAddRows = False
        Me.OnOffInterface.AllowUserToDeleteRows = False
        Me.OnOffInterface.AllowUserToOrderColumns = True
        Me.OnOffInterface.AllowUserToResizeColumns = False
        Me.OnOffInterface.AllowUserToResizeRows = False
        Me.OnOffInterface.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.OnOffInterface.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.registry, Me.windowstate})
        Me.OnOffInterface.Location = New System.Drawing.Point(12, 12)
        Me.OnOffInterface.Name = "OnOffInterface"
        Me.OnOffInterface.Size = New System.Drawing.Size(394, 269)
        Me.OnOffInterface.TabIndex = 0
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(331, 287)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 48)
        Me.cmdCancel.TabIndex = 1
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdSave
        '
        Me.cmdSave.Location = New System.Drawing.Point(250, 287)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(75, 48)
        Me.cmdSave.TabIndex = 2
        Me.cmdSave.Text = "Save"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'registry
        '
        Me.registry.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.registry.FillWeight = 149.2386!
        Me.registry.HeaderText = "Registry"
        Me.registry.Name = "registry"
        Me.registry.ReadOnly = True
        '
        'windowstate
        '
        Me.windowstate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.windowstate.FillWeight = 50.76142!
        Me.windowstate.HeaderText = "On / Off Window"
        Me.windowstate.Items.AddRange(New Object() {"ON", "OFF"})
        Me.windowstate.Name = "windowstate"
        '
        'OnOffWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(418, 346)
        Me.Controls.Add(Me.cmdSave)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.OnOffInterface)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "OnOffWindow"
        Me.Text = "OnOffWindow"
        Me.TopMost = True
        CType(Me.OnOffInterface, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents OnOffInterface As DataGridView
    Friend WithEvents cmdCancel As Button
    Friend WithEvents cmdSave As Button
    Friend WithEvents registry As DataGridViewTextBoxColumn
    Friend WithEvents windowstate As DataGridViewComboBoxColumn
End Class
