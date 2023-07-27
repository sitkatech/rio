import { EditorComponent } from "@tinymce/tinymce-angular";

export default class TinyMCEHelpers {

    /**
     * @param editorComponent 
     * @param overrideConfig 
     * @returns 
     */
    public static DefaultInitConfig(editorComponent : EditorComponent = null, overrideConfig : object = null) : object {
      debugger;
        let config = { 
            plugins: 'lists link image table code help wordcount', 
            file_picker_types: 'image',
            images_file_types: 'jpg,svg,webp,gif',
            image_title: true,
            promotion: false,
            // This is from the TinyMCE documentation on how to handle image uploads
            file_picker_callback: (cb, value, meta) => {
              const input = document.createElement('input');
              input.setAttribute('type', 'file');
              input.setAttribute('accept', 'image/*');
              input.addEventListener('change', (e) => {
                const file = (e.target as any).files[0];

                const reader = new FileReader();
                reader.addEventListener('load', () => {
                  /*
                    Note: Now we need to register the blob in TinyMCEs image blob
                    registry. In the next release this part hopefully won't be
                    necessary, as we are looking to handle it internally.
                  */
                  const id = 'blobid' + (new Date()).getTime();
                  const blobCache =  editorComponent.editor.editorUpload.blobCache;
                  const base64 = (reader.result as any).split(',')[1];
                  const blobInfo = blobCache.create(id, file, base64);
                  blobCache.add(blobInfo);

                  /* call the callback and populate the Title field with the file name */
                  cb(blobInfo.blobUri(), { title: file.name });
                });
                reader.readAsDataURL(file);
              });
              input.click();
            },
        }; 

        let resultConfig = this.OverrideConfig(config, overrideConfig)
        console.log(resultConfig)
        return resultConfig;
    }

    /**
     * This is not a fancy merge of config, it overrides all keys present in the override config.
     * 
     * @param config 
     * @param overrideConfig 
     * @returns 
     */
    private static OverrideConfig(config : object, overrideConfig: object) : object {
        if(!overrideConfig) return config;
        Object.keys(overrideConfig).forEach(configKey => {
            config[configKey] = overrideConfig[configKey];
        })
        return config;
    }

}