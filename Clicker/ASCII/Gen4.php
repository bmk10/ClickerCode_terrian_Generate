<?php
// <Action><X>588</X><Y>299</Y><Text /><interval>2</interval><Type>0</Type></Action><Action><X>580</X><Y>306</Y><Text /><interval>4</interval><Type>2</Type></Action><Action><X>577</X><Y>282</Y><Text /><interval>0</interval><Type>2</Type></Action><Action><X>573</X><Y>314</Y><Text /><interval>0</interval><Type>2</Type></Action><Action><X>572</X><Y>375</Y><Text /><interval>0</interval><Type>2</Type></Action><Action><X>572</X><Y>384</Y><Text /><interval>4</interval><Type>2</Type></Action><Action><X>544</X><Y>318</Y><Text /><interval>0</interval><Type>2</Type></Action><Action><X>544</X><Y>318</Y><Text /><interval>0</interval><Type>2</Type></Action></ActionsEntry>

$headerClicker = '<?xml version="1.0" encoding="utf-8"?><ActionsEntry xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">';
$actionTag= "<Action>";

$actionX= "<X>";
$actionY= "<Y>";
$actionText= "<Text>";
$actionInterval= "<interval>";
$actionType= "<Type>";
$actionActionsEntry = "<ActionsEntry>";
$actionTag2 = "</Action>";

$actionX2= "</X>";
$actionY2= "</Y>";
$actionText2= "</Text>";
$actionInterval2= "</interval>";
$actionType2= "</Type>";
$actionActionsEntry2 = "</ActionsEntry>";



// </X><Y>318</Y><Text /><interval>0</interval><Type>2</Type></Action></ActionsEntry>


$base_filebuffer = "";

$base_filebuffer .= $headerClicker;
for($j=0; $j  < 36; $j++)
{
for($i=0; $i  < 360; $i++)
{
// <X>572</X><Y>384</Y>

$x = (cos($i));


$y = sin($i);

$x *= 100;
$y *= 100;

$x += 500;
$y += 500;


$x = round($x)+$i;


$y = round($y);

$actionTextFieldValue = "";
$intervalTextFieldValue = "0";
$TypeTextFieldValue = "2";
$actionTextFieldValue2 = "blank";

$base_filebuffer .= $actionTag;
$base_filebuffer .= $actionX;
$base_filebuffer .= $x;
$base_filebuffer .= $actionX2;
$base_filebuffer .= $actionY;
$base_filebuffer .= $y;
$base_filebuffer .= $actionY2;

$base_filebuffer .= $actionText;
$base_filebuffer .= $actionTextFieldValue;
$base_filebuffer .= $actionText2;

$base_filebuffer .= $actionInterval;
$base_filebuffer .= $intervalTextFieldValue ;
$base_filebuffer .= $actionInterval2;

$base_filebuffer .= $actionType;
$base_filebuffer .= $TypeTextFieldValue;
$base_filebuffer .= $actionType2;

$base_filebuffer .= $actionTag2;
}
}
$base_filebuffer .= $actionActionsEntry2;


$myfile = fopen("newfile3.xml", "w") or die("Unable to open file!");
$txt = "John Doe\n";
//fwrite($myfile, $txt);
//$txt = "Jane Doe\n";
fwrite($myfile, $base_filebuffer);
fclose($myfile);

?>