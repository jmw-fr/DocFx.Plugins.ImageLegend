# DocFx.Plugins.ImageLegend

## How it works

This extension for DocFx transforms any image in the documents in html5 `<figure>` with `<figcaption>` using the `title` attribute.

For instance :

```html
<img src="myimage.png" title="MyImage">
```

will be transformed to :

``` html
<figure style="text-align: center">
    <img src="myimage.png" title="MyImage">
    <figcaption><u><em>{title}</em></u></figcaption>
</figure>
```

## How to use

Compile the project and copy the release version into `tool/plugins`directory of DocFx (`C:\ProgramData\chocolatey\lib\docfx\tools\plugins` if using chocolatey).
