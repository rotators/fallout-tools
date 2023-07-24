<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Table_Form
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
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

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Table_Form))
        Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CheckAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeselecAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.CheckedListBox6 = New System.Windows.Forms.CheckedListBox()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.CheckedListBox2 = New System.Windows.Forms.CheckedListBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.CheckedListBox3 = New System.Windows.Forms.CheckedListBox()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.CheckedListBox4 = New System.Windows.Forms.CheckedListBox()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.CheckedListBox5 = New System.Windows.Forms.CheckedListBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.SuspendLayout()
        '
        'CheckedListBox1
        '
        Me.CheckedListBox1.CheckOnClick = True
        Me.CheckedListBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox1.FormattingEnabled = True
        Me.CheckedListBox1.Items.AddRange(New Object() {"PID", "FrmID", "Damage Type", "Min Damage", "Max Damage", "Attack Primary", "Attack Secondary", "Range Primary", "Range Secondary", "AP Cost Primary", "AP Cost Secondary", "Max Ammo", "Burst Rounds", "Caliber", "Ammo PID", "Min Strength", "Critical Fail", "Cost", "Weight", "Size", "Perk", "Anim Code", "Big Gun [Flag]", "Two Hand [Flag]", "Energy [Flag]", "Shoot Thru [Flag]", "Light Thru [Flag]"})
        Me.CheckedListBox1.Location = New System.Drawing.Point(3, 3)
        Me.CheckedListBox1.Name = "CheckedListBox1"
        Me.CheckedListBox1.ScrollAlwaysVisible = True
        Me.CheckedListBox1.Size = New System.Drawing.Size(256, 306)
        Me.CheckedListBox1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckAllToolStripMenuItem, Me.DeselecAllToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(141, 48)
        '
        'CheckAllToolStripMenuItem
        '
        Me.CheckAllToolStripMenuItem.Name = "CheckAllToolStripMenuItem"
        Me.CheckAllToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.CheckAllToolStripMenuItem.Text = "Select All"
        '
        'DeselecAllToolStripMenuItem
        '
        Me.DeselecAllToolStripMenuItem.Name = "DeselecAllToolStripMenuItem"
        Me.DeselecAllToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.DeselecAllToolStripMenuItem.Text = "Deselect All"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Location = New System.Drawing.Point(3, 2)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(270, 339)
        Me.TabControl1.TabIndex = 1
        '
        'TabPage6
        '
        Me.TabPage6.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage6.Controls.Add(Me.CheckedListBox6)
        Me.TabPage6.Location = New System.Drawing.Point(4, 23)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(262, 312)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Critter"
        '
        'CheckedListBox6
        '
        Me.CheckedListBox6.CheckOnClick = True
        Me.CheckedListBox6.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox6.FormattingEnabled = True
        Me.CheckedListBox6.Items.AddRange(New Object() {"PID", "FrmID", "Strength", "Perception", "Endurance", "Charisma", "Intelligence", "Agility", "Luck", "Action Point", "Armor Class", "Health Point", "Healing Rate", "Melee Damage", "Critical Chance", "Sequence", "Exp Value", "Damage Type", "Small Guns [Skill]", "Big Guns [Skill]", "Energy Weapons [Skill]", "Unarmed [Skill]", "Melee [Skill]", "Throwing [Skill]", "First Aid [Skill]", "Doctor [Skill]", "Sneak [Skill]", "Lockpick [Skill]", "Steal [Skill]", "Traps [Skill]", "Science [Skill]", "Repear [Skill]", "Speech [Skill]", "Barter [Skill]", "Gambling [Skill]", "Outdoorsman [Skill]", "Resistance Radiation", "Resistance Poison", "Normal DT|DR", "Laser DT|DR", "Fire DT|DR", "Plasma DT|DR", "Electrical DT|DR", "Explosion DT|DR", "EMP DT|DR", "Base Normal DT|DR", "Base Laser DT|DR", "Base Fire DT|DR", "Base Plasma DT|DR", "Base Electrical DT|DR", "Base Explosion DT|DR", "Base EMP DT|DR"})
        Me.CheckedListBox6.Location = New System.Drawing.Point(3, 3)
        Me.CheckedListBox6.Name = "CheckedListBox6"
        Me.CheckedListBox6.ScrollAlwaysVisible = True
        Me.CheckedListBox6.Size = New System.Drawing.Size(256, 306)
        Me.CheckedListBox6.TabIndex = 3
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage1.Controls.Add(Me.CheckedListBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 23)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(262, 312)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Weapon"
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage2.Controls.Add(Me.CheckedListBox2)
        Me.TabPage2.Location = New System.Drawing.Point(4, 23)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(262, 312)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Ammo"
        '
        'CheckedListBox2
        '
        Me.CheckedListBox2.CheckOnClick = True
        Me.CheckedListBox2.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox2.FormattingEnabled = True
        Me.CheckedListBox2.Items.AddRange(New Object() {"PID", "Cost", "Weight", "Caliber", "Quantity", "AC Adjust", "DR Adjust", "Dam Mult", "Dam Div", "Size", "Shoot Thru [Flag]", "Light Thru [Flag]"})
        Me.CheckedListBox2.Location = New System.Drawing.Point(3, 3)
        Me.CheckedListBox2.Name = "CheckedListBox2"
        Me.CheckedListBox2.ScrollAlwaysVisible = True
        Me.CheckedListBox2.Size = New System.Drawing.Size(256, 306)
        Me.CheckedListBox2.TabIndex = 1
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage3.Controls.Add(Me.CheckedListBox3)
        Me.TabPage3.Location = New System.Drawing.Point(4, 23)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(262, 312)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Armor"
        '
        'CheckedListBox3
        '
        Me.CheckedListBox3.CheckOnClick = True
        Me.CheckedListBox3.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox3.FormattingEnabled = True
        Me.CheckedListBox3.Items.AddRange(New Object() {"PID", "FrmID", "Cost", "Weight", "Armor Class", "Normal DT|DR", "Laser DT|DR", "Fire DT|DR", "Plasma DT|DR", "Electrical DT|DR", "Explosion DT|DR", "EMP DT|DR", "Perk", "Size", "Shoot Thru [Flag]", "Light Thru [Flag]"})
        Me.CheckedListBox3.Location = New System.Drawing.Point(3, 3)
        Me.CheckedListBox3.Name = "CheckedListBox3"
        Me.CheckedListBox3.ScrollAlwaysVisible = True
        Me.CheckedListBox3.Size = New System.Drawing.Size(256, 306)
        Me.CheckedListBox3.TabIndex = 2
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage4.Controls.Add(Me.CheckedListBox4)
        Me.TabPage4.Location = New System.Drawing.Point(4, 23)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(262, 312)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Drugs"
        '
        'CheckedListBox4
        '
        Me.CheckedListBox4.CheckOnClick = True
        Me.CheckedListBox4.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox4.FormattingEnabled = True
        Me.CheckedListBox4.Items.AddRange(New Object() {"PID", "FrmID", "Cost", "Weight", "Modify Stat 0", "Modify Stat 1", "Modify Stat 2", "Instant Amount 0", "Instant Amount 1", "Instant Amount 2", "First Amount 0", "First Amount 1", "First Amount 2", "First Duration Time", "Second Amount 0", "Second Amount 1", "Second Amount 2", "Second Duration Time", "Addiction Effect", "Addiction Onset Time", "Addiction Rate", "Size", "Shoot Thru [Flag]", "Light Thru [Flag]"})
        Me.CheckedListBox4.Location = New System.Drawing.Point(3, 3)
        Me.CheckedListBox4.Name = "CheckedListBox4"
        Me.CheckedListBox4.ScrollAlwaysVisible = True
        Me.CheckedListBox4.Size = New System.Drawing.Size(256, 306)
        Me.CheckedListBox4.TabIndex = 3
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage5.Controls.Add(Me.CheckedListBox5)
        Me.TabPage5.Location = New System.Drawing.Point(4, 23)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(262, 312)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "Misc"
        '
        'CheckedListBox5
        '
        Me.CheckedListBox5.CheckOnClick = True
        Me.CheckedListBox5.ContextMenuStrip = Me.ContextMenuStrip1
        Me.CheckedListBox5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CheckedListBox5.FormattingEnabled = True
        Me.CheckedListBox5.Items.AddRange(New Object() {"PID", "FrmID", "Cost", "Weight", "Size", "Power PID", "Power Type", "Charges", "Shoot Thru [Flag]", "Light Thru [Flag]"})
        Me.CheckedListBox5.Location = New System.Drawing.Point(3, 3)
        Me.CheckedListBox5.Name = "CheckedListBox5"
        Me.CheckedListBox5.ScrollAlwaysVisible = True
        Me.CheckedListBox5.Size = New System.Drawing.Size(256, 306)
        Me.CheckedListBox5.TabIndex = 2
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.Button1.Image = CType(resources.GetObject("Button1.Image"), System.Drawing.Image)
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Button1.Location = New System.Drawing.Point(65, 345)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(149, 39)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Create Table"
        Me.Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button1.UseVisualStyleBackColor = True
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.DefaultExt = "csv"
        Me.SaveFileDialog1.Filter = "Excel (.csv)|*.csv|All Types|*.*"
        Me.SaveFileDialog1.RestoreDirectory = True
        Me.SaveFileDialog1.Title = "Export Table"
        '
        'Table_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(275, 389)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Button1)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Table_Form"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "TTX - Table"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CheckedListBox1 As System.Windows.Forms.CheckedListBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CheckAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeselecAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckedListBox2 As System.Windows.Forms.CheckedListBox
    Friend WithEvents CheckedListBox3 As System.Windows.Forms.CheckedListBox
    Friend WithEvents CheckedListBox4 As System.Windows.Forms.CheckedListBox
    Friend WithEvents CheckedListBox5 As System.Windows.Forms.CheckedListBox
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents CheckedListBox6 As System.Windows.Forms.CheckedListBox
End Class
