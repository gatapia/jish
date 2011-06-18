/*jish.assembly('js.net.jish/bin/System.Windows.Forms.dll');

var mb = jish.create('System.Windows.Forms.MessageBox');
mb.Show('Hello World');


*/

jish.assembly('js.net.jish/bin/System.Drawing.dll')
jish.assembly('js.net.jish/bin/System.Windows.Forms.dll')

var app = jish.create('System.Windows.Forms.Application');
var form = jish.create('System.Windows.Forms.Form');
var lbl = jish.create('System.Windows.Forms.Label');
form.Text = lbl.Text = 'Hello World!';
lbl.Location = jish.create('System.Drawing.Point', 50, 50);
form.Controls.Add(lbl);

app.Run(form);