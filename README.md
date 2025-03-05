就職活動用に制作した物です。
# MaterialFolderManagement
3分の紹介動画(https://1drv.ms/v/s!AoVv-EFPqdKMlbRWPUUlP6Q_NFD2ZQ?e=mHqkdN)
 Unityでファイルとフォルダを管理をする。ファイルの検索や再生、タグ付け、コピー、なども出来る様にする。  
 ※基本的に制御するフォルダ、ファイルはアプリ内のみだが、内フォルダやファイルを移動やコピーさせる時だけ外のフォルダにアクセスする。

## このプロジェクトには第三者のライブラリが含まれています。著作権表記はLICENSEフォルダをご確認ください。

# 機能一覧
## ファイル、フォルダ
* 階層表示(一つの層ずつの表情エクスプローラーみたいな感じ)。  
* フォルダ、ファイルの検索(除外、配下のファイル全て表示、五十音順等々)。  
* タグ(#)、別名、備忘録などの付与や.wavの再生。  
* ロック(ロックしていると複数選択時除外され、移動とコピペが出来ない。)※単独選択時にはタグや備忘録等々は出来るが移動、コピペは出来ない。
* 移動、コピー。(同名の場合、上書き、その場で名前の変更、"別名"に変更又はその逆、キャンセルから選べるようにする)。
* 移動、コピー時に別名表記に変更する機能。
## 検索機能  
* 検索は現在開いている層とその配下のファイル、フォルダを調べるのが基本的な動作。
* ファイル、フォルダ名に含まれる文字列での検索。その他、別名、備忘録、タグの文字列を含むか選択可。
* 限定検索。タグでの検索、備忘録の文字列の検索、別名の文字列で検索。
* 除外検索。
* 検索結果は五十音順や日付順で表示する(昇順、降順切り替えられるようにする)。
## パンくずリスト
* パンくずリストの表示
* 矢印ボタンを配置する。←は常に一つ上の階層に移動する。→は直近で開いていた最下層まで一つの層ずつたどる。
## フォルダのピン止め(ショートカット)
* エクスプローラーのピン止めと同じ。

