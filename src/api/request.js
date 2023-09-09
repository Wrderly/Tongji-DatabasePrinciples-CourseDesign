// 对axios进行二次封装
import axios from 'axios'

// requests就是axios，只不过稍微配置一下
const requests  = axios.create({
    // 配置对象
    //基础路径，发送请求时，路径当中会出现api
    baseURL:'/api',
    // 代表请求超时的时间
    timeout:5000
})
// 请求拦截器：发送请求前做一些事情
requests.interceptors.request.use((config)=>{
    // config:配置对象，headers请求头很重要
    return config;
})

requests.interceptors.response.use((res)=>{
    // 成功的回调函数：服务器响应数据回来以后，响应拦截器可以检测到，可以做一些事情
    return res.data;

},(err)=>{
    // 响应失败的回调函数
    return Promise.reject(new Error('fail'))
})


export default requests;