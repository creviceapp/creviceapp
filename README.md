| master | develop |
|--------|---------|
| [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) |


**Crevice4** is multi purpose utility which supports gestures with **mouse** and **keyboard**.

You can use **C# language** in your customizable userscript file, so there is **nothing that can not be done for you**.

Gestures for standard browsers are built in, by default.
</p>

## Documentation

[https://creviceapp.github.io](https://creviceapp.github.io)

## Lisence

MIT Lisense

## Author

<style>
@font-face {
  font-family: 'icomoon';
  src:  url('https://creviceapp.github.io/fonts/icomoon.eot?ohsdgx');
  src:  url('https://creviceapp.github.io/fonts/icomoon.eot?ohsdgx#iefix') format('embedded-opentype'),
    url('https://creviceapp.github.io/fonts/icomoon.ttf?ohsdgx') format('truetype'),
    url('https://creviceapp.github.io/fonts/icomoon.woff?ohsdgx') format('woff'),
    url('https://creviceapp.github.io/fonts/icomoon.svg?ohsdgx#icomoon') format('svg');
  font-weight: normal;
  font-style: normal;
}

[class^="icon-"], [class*=" icon-"] {
  /* use !important to prevent issues with browser extensions that change fonts */
  font-family: 'icomoon' !important;
  speak: none;
  font-style: normal;
  font-weight: normal;
  font-variant: normal;
  text-transform: none;
  line-height: 1;

  /* Better Font Rendering =========== */
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.icon-github:before {
  content: "\e901";
}
.icon-twitter:before {
  content: "\ea96";
}

.avator { 
    display: flex;
    justify-content: left;
    align-items: center;
    position: relative;
    width: 100%;
}

.avator .github-avator {
    border-radius: 15%;
    position: relative;
    max-width: 20px;
    min-width: 20px;
    vertical-align: text-bottom;
}

.avator .github-username {
    position: relative;
    margin-left: 5px;
}

.avator .icon-outer:before {
  padding-top: 100%;
  display: block;
  content: "";
}
.avator .icon-outer {
  position: relative;
  width: 20%;
  max-width: 40px;
  min-width: 50px;
  margin-left: 10px;
}

.avator .icon-inner {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  border: 1px solid #7f7f7f;
  border-radius: 50%;
  transition: all 0.2s ease;
  cursor: pointer;
}

.avator .icon-inner-github {
    border-color: #B083DF;
    color: #B083DF;
    background-color: #ffffff;
}
.avator .icon-inner-github:hover {
    background-color: #B083DF;
    color: #ffffff;
}

.avator .icon-inner-twitter {
    border-color: #29b6f6;
    color: #29b6f6;
    background-color: #ffffff;
}
.avator .icon-inner-twitter:hover {
    background-color: #29b6f6;
    color: #ffffff;
}

.avator .icon {
  font-size: 24px;
}
</style>

<div class="avator">
    <span><img class="github-avator" src="https://avatars2.githubusercontent.com/u/841131?s=40&amp;v=4" alt="@rubyu"></span>
    <span class="github-username">rubyu</span>
    <div class="icon-outer icon-outer-github">
        <a href="https://github.com/rubyu"><span class="icon-inner icon-inner-github"><span class="icon icon-github "></span></span></a>
    </div>
    <div class="icon-outer icon-outer-twitter">
        <a href="https://twitter.com/ruby_u"><span class="icon-inner icon-inner-twitter"><span class="icon icon-twitter"></span></span></a>
    </div>
</div>
