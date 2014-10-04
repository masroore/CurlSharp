<?php
foreach (apache_request_headers() as $header => $value)
{
  echo "$header: $value\n";
}
echo "\r\n";
print_r($_POST);