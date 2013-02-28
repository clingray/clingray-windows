using ClingClient.controls;
namespace ClingClient.commitList {
    partial class MonthSeparatorBar {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent() {
            this.monthLabel = new ClingClient.controls.ShadowLabel();
            this.SuspendLayout();
            // 
            // monthLabel
            // 
            this.monthLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.monthLabel.Font = new System.Drawing.Font("굴림", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.monthLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(232)))), ((int)(((byte)(237)))));
            this.monthLabel.Location = new System.Drawing.Point(201, 8);
            this.monthLabel.Name = "monthLabel";
            this.monthLabel.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.monthLabel.Size = new System.Drawing.Size(44, 10);
            this.monthLabel.TabIndex = 0;
            this.monthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.monthLabel.xOffsetShadow = 0F;
            this.monthLabel.yOffsetShadow = -1F;
            // 
            // MonthSeparatorBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackgroundImage = global::ClingClient.Properties.Resources.img_list_month;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.monthLabel);
            this.DoubleBuffered = true;
            this.Name = "MonthSeparatorBar";
            this.Size = new System.Drawing.Size(445, 25);
            this.ResumeLayout(false);

        }

        #endregion

        private ShadowLabel monthLabel;
    }
}
