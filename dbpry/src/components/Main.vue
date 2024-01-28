<script>
import Layout from './Layout.vue';
import Editor from './Editor.vue';

export default {
  components: {
    Layout: Layout,
    Editor: Editor,
  },
  data() {
    return {
      src: `function foo(text) {
  console.log(text);
}

foo(1234);

`,
      logText: "",
    }
  },
  methods: {
    msg(text) {
      console.log(text);
    },
    outToLog(msg, prefix = "") {
      if(this.logText !== "") {
        this.logText = this.logText + "\n";
      }

      let isObject = (value) => {
        return value !== null && typeof value === 'object'
      };

      this.logText = this.logText + prefix + ((isObject(msg))? JSON.stringify(msg) : msg);
    },
    resetJavaScriptContext() {
      let target = this.$refs.sandboxes.querySelector('iframe[name="default"]');
      if(target) {
        target.remove();
      }
      let iframe = document.createElement('iframe');
      iframe.name = "default";
      this.$refs.sandboxes.appendChild(iframe);

      iframe.contentWindow.consoleListener = (msg) => {
        this.outToLog(msg);
      };

      iframe.contentWindow.querySync = function(sql) {
        const request = new XMLHttpRequest();
        const endpoint = 'https://localhost:7016';
        const param = (new URLSearchParams({query: sql})).toString();
        request.open("GET", `${endpoint}/Database?${param}`, false);
        request.send(null);
        
        if (request.status === 200) {
          return JSON.parse(request.responseText);
        } else {
          throw new Error("Error on execute query");
        }
      };

      iframe.contentWindow.eval(`
        window.console.__log = console.log;
        window.console.log = (arg) => { console.__log(arg); consoleListener(arg); }
        `);
    },
    evaluateJavaScript(text) {
      // const context = window; //{ ...window };
      // // const func = new Function(...Object.keys(context), `return (() => { ${text} })()`);
      // const func = new Function(...Object.keys(context), `${text}`);
      // // console.log(func(...Object.values(context)));
      // console.log(func.apply(window, ...Object.values(context)));

      // eval.apply(window, [text]);

      // const run = Function(text);
      // run();

      // const run = (code) => Function(`return (functions) => { ${code} }`)()(functions);
      // run();

      // const context = { bar : 123 };
      // const run = new Function(text);
      // run.apply(context, []);

      // let run = new Function(text);
      // run();


    //<iframe ref="sandbox" style="display:none"></iframe>

      // this.$refs.sandboxes.contentWindow.eval(text);
      let context = this.$refs.sandboxes.querySelector('iframe[name="default"]');
      try {
        let result = context.contentWindow.eval(text);
        this.outToLog(result, "> ");
        console.log(result);
      } catch(err) {
        this.outToLog(err.toString(), "ERROR: ");
        console.error(err);
      }
    }
  },
  mounted() {
    this.resetJavaScriptContext();
  }
}
</script>

<template>
  <div ref="sandboxes" style="display:none">
  </div>
  <Layout layout-id="layout-outer"
    border-width="4"
    size-north="48" size-west="24" size-south="24" size-east="100"
    class-north="p-3" class-south="p-3 bg-forbidden" class-east="p-3" class-west="p-3 bg-forbidden" class-center="p-3"
    disable-east="true" disable-west="true"
    resizable-north="false" resizable-south="false" >
    <template v-slot:north>
      <img src="../assets/dbpry-icon.svg" class="logo" alt="dbpry logo" height="50" style="margin-top:-2px; transform:rotate(0deg);" />
      <span style="position:absolute; top:20px; left:20px;">
        <span style="color:#FFF">db</span><span style="margin-left:2px">pry</span>
      </span>
    </template>

    <template v-slot:west>
      <!-- tools and components -->
    </template>

    <template v-slot:center>

      <Layout layout-id="layout-inner"
        border-width="4"
        size-north="24"
        disable-east="true" disable-west="true" disable-south="true" disable-north="false"
        class-north="p-3 bg-forbidden" class-south="p-3" class-east="p-3" class-west="p-3" class-center="p-3"
        resizable-north="false" >
        <template v-slot:north>
          <!-- tabbar -->
        </template>

        <template v-slot:center>

          <!-- tab page 1 -->
          <Layout layout-id="layout-content1"
            border-width="4"
            size-east="300"
            class-north="p-3" class-south="p-3" class-east="p-3 bg-forbidden" class-west="p-3" class-center="p-3"
            disable-west="true" disable-north="true" disable-south="true"
            disable-east="true">

            <template v-slot:center>
              <Layout layout-id="layout-content1-inner1"
                border-width="4"
                size-north="32" size-south="150"
                class-south="p-3" class-east="p-3" class-west="p-3" class-center="p-3"
                disable-west="true" disable-east="true" >
                <template v-slot:north>
                  <div class="button-container">
                    <button class="button1" @click="evaluateJavaScript($refs.editor.editor.getSelectedText())">
                      Evaluate<span style="font-size:0.7em;">&nbsp;(Ctrl+Enter)</span></button>
                    <button class="button1" @click="resetJavaScriptContext()">Reset</button>
                  </div>
                </template>
                <template v-slot:center>
                  <Editor ref="editor" editorId="editor1" :content="src" theme="textmate" lang="javascript"
                    @keydown.ctrl.enter="evaluateJavaScript($refs.editor.editor.getSelectedText())">
                  </Editor>
                </template>
                <template v-slot:south>
                  <Editor editorId="editor2" :content="logText" theme="textmate" lang="text">
                  </Editor>
                </template>
              </Layout>
            </template>
          </Layout>

        </template>
      </Layout>

      <!-- tab page -->
      <!--
      <Layout
        layout-id="layout-inner-center"
        border-width="4"
        size-north="24"
        size-west="250"
        size-south="100"
        size-east="300"
        disable-west="true"
        >
        <template v-slot:center>
          editor
        </template>
        
        <template v-slot:east>
          viewer
        </template>

        <template v-slot:south>
          log
        </template>
      </Layout>
      -->

    </template>

    <template v-slot:south>
      
    </template>
  </Layout>
</template>

<style scoped>
</style>
