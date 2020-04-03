namespace ExcelReader
{
	partial class CsvReader
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.inputSaveScript = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.listFiles = new System.Windows.Forms.ListBox();
			this.btnParse = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "保存脚本至：";
			// 
			// inputSaveScript
			// 
			this.inputSaveScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.inputSaveScript.Location = new System.Drawing.Point(150, 6);
			this.inputSaveScript.Name = "inputSaveScript";
			this.inputSaveScript.Size = new System.Drawing.Size(1099, 35);
			this.inputSaveScript.TabIndex = 1;
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(1254, 6);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(141, 35);
			this.btnSave.TabIndex = 2;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// listFiles
			// 
			this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listFiles.FormattingEnabled = true;
			this.listFiles.ItemHeight = 24;
			this.listFiles.Location = new System.Drawing.Point(16, 54);
			this.listFiles.Name = "listFiles";
			this.listFiles.Size = new System.Drawing.Size(1379, 628);
			this.listFiles.TabIndex = 3;
			// 
			// btnParse
			// 
			this.btnParse.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnParse.Location = new System.Drawing.Point(635, 688);
			this.btnParse.Name = "btnParse";
			this.btnParse.Size = new System.Drawing.Size(160, 35);
			this.btnParse.TabIndex = 4;
			this.btnParse.Text = "Parse";
			this.btnParse.UseVisualStyleBackColor = true;
			this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
			// 
			// CsvReader
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1407, 741);
			this.Controls.Add(this.btnParse);
			this.Controls.Add(this.listFiles);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.inputSaveScript);
			this.Controls.Add(this.label1);
			this.Name = "CsvReader";
			this.Text = "CsvReader";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CsvReader_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CsvReader_DragEnter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox inputSaveScript;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.ListBox listFiles;
		private System.Windows.Forms.Button btnParse;
	}
}

