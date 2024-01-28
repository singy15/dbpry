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

console.log(1+2);
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
      this.logText = this.logText + prefix + JSON.stringify(msg);
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
      let result = context.contentWindow.eval(text);
      this.outToLog(result, "> ");
      console.log(result);
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
    size-north="48" size-west="150" size-south="24" size-east="100"
    disable-east="true"
    resizable-north="false" resizable-south="false" >
    <template v-slot:north>
      <span>dbpry</span>
    </template>

    <template v-slot:west>
      tools and components
    </template>

    <template v-slot:center>

      <Layout layout-id="layout-inner"
        border-width="4"
        size-north="24"
        disable-east="true" disable-west="true" disable-south="true"
        resizable-north="false" >
        <template v-slot:north>
          tabbar
        </template>

        <template v-slot:center>

          <Layout layout-id="layout-content1"
            border-width="4"
            size-east="300"
            disable-west="true" disable-north="true" disable-south="true" >

            <template v-slot:center>
              <Layout layout-id="layout-content1-inner1"
                border-width="4"
                size-north="30" size-south="100"
                disable-west="true" disable-east="true" >
                <template v-slot:north>
                  <div class="button-container">
                    <button class="button1" @click="resetJavaScriptContext()">Reset</button>
                    <button class="button1" @click="evaluateJavaScript($refs.editor.editor.getSelectedText())">
                      Evaluate</button>
                  </div>
                </template>
                <template v-slot:center>
                  <Editor ref="editor" editorId="editor1" :content="src" theme="monokai" lang="javascript"
                    @keydown.ctrl.enter="evaluateJavaScript($refs.editor.editor.getSelectedText())">
                  </Editor>
                </template>
                <template v-slot:south>
                  <Editor editorId="editor2" :content="logText" theme="monokai" lang="text">
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
      status
    </template>
  </Layout>
</template>

<style scoped>
</style>
